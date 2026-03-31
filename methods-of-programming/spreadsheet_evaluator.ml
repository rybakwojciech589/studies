type bop =
  (* arithmetic *)
  | Add | Sub | Mult | Div
  (* logic *)
  | And | Or
  (* comparison *)
  | Eq | Neq | Leq | Lt | Geq | Gt


type ident = string

type expr =
  | Int    of int
  | Binop  of bop * expr * expr
  | Bool   of bool
  | If     of expr * expr * expr
  | Let    of ident * expr * expr
  | Var    of ident
  | Cell   of int * int
  | Unit
  | Pair   of expr * expr
  | Fst    of expr
  | Snd    of expr
  | Match  of expr * ident * ident * expr
  | IsPair of expr
  | Fun    of ident * expr
  | Funrec of ident * ident * expr
  | App    of expr * expr

type env = value Map.Make(String).t

and value =
  | VInt of int
  | VBool of bool
  | VUnit
  | VPair of value * value
  | VClosure of ident * expr * env
  | VRecClosure of ident * ident * expr * env

module M = Map.Make(String)

let eval_op (op : bop) (val1 : value) (val2 : value) : value =
  match op, val1, val2 with
  | Add,  VInt  v1, VInt  v2 -> VInt  (v1 + v2)
  | Sub,  VInt  v1, VInt  v2 -> VInt  (v1 - v2)
  | Mult, VInt  v1, VInt  v2 -> VInt  (v1 * v2)
  | Div,  VInt  v1, VInt  v2 -> VInt  (v1 / v2)
  | And,  VBool v1, VBool v2 -> VBool (v1 && v2)
  | Or,   VBool v1, VBool v2 -> VBool (v1 || v2)
  | Leq,  VInt  v1, VInt  v2 -> VBool (v1 <= v2)
  | Lt,   VInt  v1, VInt  v2 -> VBool (v1 < v2)
  | Gt,   VInt  v1, VInt  v2 -> VBool (v1 > v2)
  | Geq,  VInt  v1, VInt  v2 -> VBool (v1 >= v2)
  | Neq,  _,        _        -> VBool (val1 <> val2)
  | Eq,   _,        _        -> VBool (val1 = val2)
  | _,    _,        _        -> failwith "type error"

let spreadsheet_data : expr list list ref = ref []
let spreadsheet_size : int ref = ref 0

let get_expr_at (m : int) (n : int) : expr =
  let row = List.nth !spreadsheet_data m in
  List.nth row n

let rec eval_env (env : env) (e : expr) : value =
  match e with
  | Int i -> VInt i
  | Bool b -> VBool b
  | Binop (op, e1, e2) ->
      eval_op op (eval_env env e1) (eval_env env e2)
  | If (b, t, e) ->
      (match eval_env env b with
        | VBool true -> eval_env env t
        | VBool false -> eval_env env e
        | _ -> failwith "type error")
  | Var x ->
      (match M.find_opt x env with
        | Some v -> v
        | None -> failwith "unknown var")
  | Let (x, e1, e2) ->
      eval_env (M.add x (eval_env env e1) env) e2
  | Pair (e1, e2) -> VPair (eval_env env e1, eval_env env e2)
  | Unit -> VUnit
  | Fst e ->
      (match eval_env env e with
        | VPair (v1, _) -> v1
        | _ -> failwith "Type error")
  | Snd e ->
      (match eval_env env e with
        | VPair (_, v2) -> v2
        | _ -> failwith "Type error")
  | Match (_e1, _x, _y, _e2) ->
      failwith "Not implemented"
  | IsPair e ->
      (match eval_env env e with
        | VPair _ -> VBool true
        | _ -> VBool false)
  | Fun (x, e) -> VClosure (x, e, env)
  | Funrec (f, x, e) -> VRecClosure (f, x, e, env)
  | App (e1, e2) ->
      let v1 = eval_env env e1 in
      let v2 = eval_env env e2 in
      (match v1 with
        | VClosure (x, body, clo_env) ->
            eval_env (M.add x v2 clo_env) body
        | VRecClosure (f, x, body, clo_env) as c ->
            eval_env (clo_env |> M.add x v2 |> M.add f c) body
        | _ -> failwith "not a function")
  | Cell (row, col) ->
      eval_env env (get_expr_at row col)

let rec is_cyclic_cell (e : expr) (depth : int) =
  if depth > !spreadsheet_size then true else
  match e with
  | Int _ | Bool _ | Var _ | Unit -> false
  | Binop (_, e1, e2)
  | Pair (e1, e2)
  | App (e1, e2) ->
      is_cyclic_cell e1 depth || is_cyclic_cell e2 depth
  | If (b, t, e) ->
      is_cyclic_cell b depth
      || is_cyclic_cell t depth
      || is_cyclic_cell e depth
  | Let (_, e1, e2) ->
      is_cyclic_cell e1 depth || is_cyclic_cell e2 depth
  | Fst e
  | Snd e
  | IsPair e
  | Fun (_, e)
  | Funrec (_, _, e) -> is_cyclic_cell e depth
  | Match (_e1, _x, _y, _e2) ->
      failwith "Not implemented"
  | Cell (row, col) -> is_cyclic_cell (get_expr_at row col) (depth + 1)

let rec is_cyclic_row (data : expr list) : bool =
  match data with
  | [] -> false
  | x :: rest ->
      (is_cyclic_cell x 0) || is_cyclic_row rest

let rec is_cyclic_whole (data : expr list list) : bool =
  match data with
  | [] -> false
  | x :: rest ->
      (is_cyclic_row x) || is_cyclic_whole rest

let rec eval_row_non_cyclic (s : expr list) : value list = (match s with
  | x :: rest -> (match rest with 
    | [] -> [(eval_env M.empty x)]
    | _ -> (eval_env M.empty x) :: eval_row_non_cyclic rest)
  | [] -> failwith "error"
)

let rec eval_spreadsheet_non_cyclic (s : expr list list) : value list list = (match s with
  | x :: rest -> (match rest with 
    | [] -> [(eval_row_non_cyclic x)]
    | _ -> (eval_row_non_cyclic x)  :: (eval_spreadsheet_non_cyclic rest))
  | [] -> failwith "error"
)

let eval_spreadsheet (s : expr list list) : value list list option =
  spreadsheet_data := s;
  let rows = List.length s in
  let cols = if rows = 0 then 0 else List.length (List.hd s) in
  spreadsheet_size := rows * cols;
  
  if is_cyclic_whole s then None 
  else Some (eval_spreadsheet_non_cyclic s)
