let alpha_num = 3
let alpha_denom = 4

type 'a tree = 
  | Leaf 
  | Node of 'a tree * 'a * 'a tree

type 'a sgtree = {
  tree : 'a tree;
  size : int;
  max_size : int
}

let alpha_height (n : int) : int =
  let ratio = float_of_int alpha_denom /. float_of_int alpha_num in
  int_of_float (floor (log (float_of_int n) /. log ratio))

let tree_to_list tree =
  let rec aux t acc =
    match t with
    | Leaf -> acc
    | Node (l, v, r) -> aux l (v :: aux r acc)
  in
  aux tree []

let split_list_at_middle (lst : 'a list) : 'a list * 'a * 'a list =
  let count = List.length lst in 
  let rec loop p left right : 'a list * 'a * 'a list = 
    if p >= (count - 1) / 2 then
      (List.rev left, List.hd right, List.tl right)
    else
      loop (p + 1) ((List.hd right) :: left) (List.tl right)
  in
  loop 0 [] lst

let rebuild_balanced (t : 'a tree) : 'a tree =
  let rec build_tree (lst : 'a list) : 'a tree =
    match lst with
    | [] -> Leaf
    | _ ->
      let (l, v, r) = split_list_at_middle lst in
      Node (build_tree l, v, build_tree r)
  in
  build_tree (tree_to_list t)

let empty : 'a sgtree = { tree = Leaf; size = 0; max_size = 0 }

let find (x : 'a) (sgt : 'a sgtree) : bool =
  let rec find_in_tree (y : 'a) (subtree : 'a tree) : bool =
    match subtree with
    | Leaf -> false
    | Node (left, root, right) ->
        root = y
        || if y > root then find_in_tree y right else find_in_tree y left
  in
  find_in_tree x sgt.tree

let rec size (x : 'a tree) : int =
  match x with 
  | Leaf -> 0
  | Node (l, _, r) -> size l + size r + 1

let needs_rebalance ll lr =
  ll * alpha_denom > (ll + lr + 1) * alpha_num
  || lr * alpha_denom > (ll + lr + 1) * alpha_num

let rec insert_into_tree (y : 'a) (subtree : 'a tree) (maxi : int) (depth : int) : 'a tree * int =
  match subtree with
  | Leaf ->
      if depth <= maxi then Node (Leaf, y, Leaf), 0
      else Node (Leaf, y, Leaf), 1
  | Node (left, root, right) ->
      if y > root then
        let (a, b) = insert_into_tree y right maxi (depth + 1) in
        if b = 0 then
          (Node (left, root, a), b)
        else
          match a with
          | Leaf -> failwith "Leaf in insert"
          | Node (l, k, p) ->
              let ll = size l and lr = size p in
              if needs_rebalance ll lr then
                (Node (left, root, rebuild_balanced a), 0)
              else
                (Node (left, root, a), 1)
      else
        let (a, b) = insert_into_tree y left maxi (depth + 1) in
        if b = 0 then 
          (Node (a, root, right), b)
        else
          match a with
          | Leaf -> failwith "Leaf in insert"
          | Node (l, k, p) ->
              let ll = size l and lr = size p in
              if needs_rebalance ll lr then
                (Node (rebuild_balanced a, root, right), 0)
              else
                (Node (a, root, right), 1)

let insert (x : 'a) (sgt : 'a sgtree) : 'a sgtree =
  if find x sgt then failwith "Element already exists in the tree"
  else
    let (new_tree, _) = insert_into_tree x sgt.tree (alpha_height (sgt.size + 1)) 0 in
    {
      tree = new_tree;
      size = sgt.size + 1;
      max_size = max (sgt.size + 1) sgt.max_size
    }

let rec min_value tree =
  match tree with
  | Leaf -> failwith "Called min_value on an empty tree"
  | Node (Leaf, value, _) -> value  
  | Node (left, _, _) -> min_value left 

let rec delete (y : 'a) (subtree : 'a tree) : 'a tree =
  match subtree with
  | Leaf -> Leaf
  | Node (left, v, right) ->
      if y < v then Node (delete y left, v, right)
      else if y > v then Node (left, v, delete y right)
      else
        match (left, right) with
        | Leaf, Leaf -> Leaf
        | Leaf, _ -> right
        | _, Leaf -> left
        | _ ->
            let min_val = min_value right in
            Node (left, min_val, delete min_val right)

let remove (x : 'a) (sgt : 'a sgtree) : 'a sgtree =
  if not (find x sgt) then failwith "Element not found in the tree"
  else
    let new_tree = delete x sgt.tree in
    if sgt.max_size * alpha_num <= (sgt.size - 1) * alpha_denom then
      {
        tree = new_tree;
        size = sgt.size - 1;
        max_size = sgt.max_size
      }
    else
      {
        tree = rebuild_balanced new_tree;
        size = sgt.size - 1;
        max_size = sgt.size - 1
      }
