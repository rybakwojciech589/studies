open Syntax

let rec wft delta t =
  (match t with
  | Var x -> List.mem x delta
  | Func (_, args) -> List.for_all (wft delta) args)

let rec wff delta f =
  (match f with
  | False -> true
  | Rel (_, ts) -> List.for_all (wft delta) ts
  | Imp (f1, f2)
  | And (f1, f2)
  | Or (f1, f2) -> wff delta f1 && wff delta f2
  | Forall (x, f) -> wff (x :: delta) f
  | ForallRel (_, f) -> wff delta f
  | Exists (x, f) -> wff (x :: delta) f)

let ill_formed_formula delta f = not (wff delta f)
let ill_formed_term delta t = not (wft delta t)

let assert_formula_equal pos f1 f2 msg =
  if not (Formula.equal f1 f2) then
    raise (Type_error(pos, msg))

let rec infer_expr delta gamma e =
  match e.data with
  | EVar x ->
      (match List.assoc_opt x gamma with
       | Some f -> f
       | None -> raise (Type_error(e.pos, "Unknown variable: " ^ x)))
  | EFun (x, f, e1) ->
      if ill_formed_formula delta f then
        raise (Type_error(e.pos, "Ill-formed formula in function abstraction"))
      else
        let gamma' = (x, f) :: gamma in
        let f1 = infer_expr delta gamma' e1 in
        Imp(f, f1)
  | EApp (e1, e2) ->
      (match infer_expr delta gamma e1 with
       | Imp(fa, fr) ->
           let fa' = infer_expr delta gamma e2 in
           assert_formula_equal e.pos fa fa' "Argument type mismatch in application";
           fr
       | _ -> raise (Type_error(e.pos, "Expression is not a function")))
  | ETermFun (x, e1) ->
      let delta' = x :: delta in
      let f1 = infer_expr delta' gamma e1 in
      Forall(x, f1)
  | ETermApp (e1, t) ->
      if ill_formed_term delta t then
        raise (Type_error(e.pos, "Ill-formed term in term application"))
      else
        (match infer_expr delta gamma e1 with
         | Forall(x, f) ->
             Formula.subst x t f
         | _ -> raise (Type_error(e.pos, "Application of a non-universal expression")))
  | ERelApp (e1, x, f) ->
      if ill_formed_formula (x :: delta) f then
        raise (Type_error(e.pos, "Ill-formed formula in relational application"))
      else
        (match infer_expr delta gamma e1 with
         | ForallRel(r, f1) ->
             Formula.subst_rel r (x, f) f1
         | _ -> raise (Type_error(e.pos, "Application of a non-universal relation")))
  | EPair (e1, e2) ->
      let f1 = infer_expr delta gamma e1 in
      let f2 = infer_expr delta gamma e2 in
      And(f1, f2)
  | EFst e1 ->
      (match infer_expr delta gamma e1 with
       | And(f1, _) -> f1
       | _ -> raise (Type_error(e.pos, "Fst on a non-pair")))
  | ESnd e1 ->
      (match infer_expr delta gamma e1 with
       | And(_, f2) -> f2
       | _ -> raise (Type_error(e.pos, "Snd on a non-pair")))
  | ELeft (e1, f2) ->
      if ill_formed_formula delta f2 then
        raise (Type_error(e.pos, "Annotation is not a valid disjunction (left side)"))
      else
        let f1 = infer_expr delta gamma e1 in
        (match f2 with
         | Or (expected_left, _) ->
             assert_formula_equal e.pos f1 expected_left "Left: argument type does not match the left side of the disjunction";
             f2
         | _ ->
             raise (Type_error(e.pos, "Left: annotation is not a disjunction")))
  | ERight (e1, f2) ->
      if ill_formed_formula delta f2 then
        raise (Type_error(e.pos, "Annotation is not a valid disjunction (right side)"))
      else
        let f1 = infer_expr delta gamma e1 in
        (match f2 with
         | Or (_, expected_right) ->
             assert_formula_equal e.pos f1 expected_right "Right: proof type does not match the right side of the disjunction";
             f2
         | _ ->
             raise (Type_error(e.pos, "Right: annotation is not a disjunction")))
  | ECase (e0, x1, e1, x2, e2) ->
      (match infer_expr delta gamma e0 with
       | Or(f1, f2) ->
           let gamma1 = (x1, f1) :: gamma in
           let gamma2 = (x2, f2) :: gamma in
           let r1 = infer_expr delta gamma1 e1 in
           let r2 = infer_expr delta gamma2 e2 in
           assert_formula_equal e.pos r1 r2 "Both branches of case must have the same type";
           r1
       | _ -> raise (Type_error(e.pos, "Case on a non-disjunction")))
  | EAbsurd (e1, f) ->
      if ill_formed_formula delta f then
        raise (Type_error(e.pos, "Ill-formed formula in absurd"))
      else
        (match infer_expr delta gamma e1 with
         | False -> f
         | Imp(f1, False) ->
             assert_formula_equal e.pos f1 f "Absurd: argument type does not match the annotation";
             False
         | _ -> raise (Type_error(e.pos, "Absurd on a non-False or non-negation")))
  | EPack (t, e1, f) ->
      if ill_formed_term delta t then
        raise (Type_error(e.pos, "Ill-formed term in pack"))
      else if ill_formed_formula delta f then
        raise (Type_error(e.pos, "Ill-formed formula in pack"))
      else
        (match f with
         | Exists(x, f1) ->
             let expected = Formula.subst x t f1 in
             let got = infer_expr delta gamma e1 in
             assert_formula_equal e.pos expected got "Pack: proof does not match the formula after substitution";
             f
         | _ ->
             raise (Type_error(e.pos, "Pack: annotation is not an existential")))
  | EUnpack (x, y, e1, e2) ->
      let f1 = infer_expr delta gamma e1 in
      (match f1 with
       | Exists(x', f2) ->
           let f2' = Formula.subst x' (Var x) f2 in
           let delta' = x :: delta in
           let gamma' = (y, f2') :: gamma in
           let f3 = infer_expr delta' gamma' e2 in
           if ill_formed_formula delta f3 then
             raise (Type_error(e.pos, "Ill-formed formula in unpack"))
           else f3
       | _ -> raise (Type_error(e.pos, "Unpack on a non-existential")))
  | ELet (x, e1, e2) ->
      let f1 = infer_expr delta gamma e1 in
      infer_expr delta ((x, f1) :: gamma) e2

let check_def delta gamma = function
  | Axiom (pos, x, f) ->
      if ill_formed_formula delta f then
        raise (Type_error(pos, "Ill-formed formula in axiom"))
      else (delta, (x, f) :: gamma)
  | Theorem (pos, x, f, e) ->
      if ill_formed_formula delta f then
        raise (Type_error(pos, "Ill-formed formula in theorem"))
      else
        let got = infer_expr delta gamma e in
        assert_formula_equal pos f got "Proof does not match the theorem statement";
        (delta, (x, f) :: gamma)

let check_defs defs =
  let rec aux delta gamma = function
    | [] -> ()
    | d :: ds ->
        let (delta', gamma') = check_def delta gamma d in
        aux delta' gamma' ds
  in
  aux [] [] defs
