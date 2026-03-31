# Methods of Programming

This repository contains a collection of advanced functional programming projects implemented in OCaml. These assignments were completed as part of the "Methods of Programming" course and focus on exploring complex data structures, language interpretation, and type theory.

Below you will find detailed summaries of the key implementations included in this repository.

---

## Formal Logic Proof Checker / Type Inference Engine (proof_checker.ml)

**Problem Summary:** The project involves implementing a proof verifier or type checker for a formal logical system, likely based on intuitionistic logic and the Curry-Howard correspondence. The goal is to verify whether a given program (proof expression) correctly inhabits a specific type (logical formula) within a given context. The system must support complex logical constructs, including propositional connectives (AND, OR, NOT/Absurdity, Implication) and quantifiers (Universal and Existential).

**Solution:**
The solution is implemented in OCaml, leveraging its strong pattern-matching capabilities and algebraic data types to process the Abstract Syntax Tree (AST) of the logic system.
* **Well-Formedness Validation:** The `wft` (well-formed term) and `wff` (well-formed formula) functions ensure that all variables used in terms and formulas are properly bound in the current evaluation context (`delta`).
* **Type Inference and Proof Verification:** The core of the system is the recursive `infer_expr` function. It evaluates proof expressions and infers their resulting logical formulas. It strictly enforces typing rules, throwing descriptive `Type_error` exceptions if a proof step is invalid (e.g., applying `Fst` to a non-pair, or a mismatched argument type in a function application).
* **Curry-Howard Isomorphism Implementation:** The code directly translates logical rules into programmatic constructs:
  * *Implication* is handled via function abstraction (`EFun`) and application (`EApp`).
  * *Conjunction (AND)* is represented by pairs (`EPair`, `EFst`, `ESnd`).
  * *Disjunction (OR)* is managed through sum types (`ELeft`, `ERight`, and `ECase` for branching).
  * *Falsum / Absurdity* is implemented using the principle of explosion (`EAbsurd`).
  * *Quantifiers* are handled via dependent typing concepts (e.g., `EPack` and `EUnpack` for existentials, `ETermFun` for universals).
* **Theorem Validation:** The `check_defs` function processes a sequence of top-level definitions (`Axiom` and `Theorem`). It verifies that each theorem's provided proof accurately yields the stated formula, progressively building a verified global context (`gamma`).

## Functional Spreadsheet Engine (spreadsheet_evaluator.ml)

**Problem Summary:** The project involves building an evaluation engine for a spreadsheet where cells contain more than just simple formulas—they contain expressions from a Turing-complete, functional programming language (a subset of ML). The system must be able to evaluate a 2D grid of these expressions, handle cell-to-cell references, support first-class functions (closures), and safely detect circular dependencies between cells to prevent infinite evaluation loops.

**Solution:**
The solution is implemented in OCaml as an Abstract Syntax Tree (AST) interpreter extended with spreadsheet-specific logic and cycle detection.
* **Mini-ML Interpreter:** The core is a recursive evaluator (`eval_env`) that processes an AST (`expr`). It supports arithmetic and boolean logic, conditional branching (`If`), local bindings (`Let`), tuples (`Pair`), and first-class functions with lexical scoping (`Fun`, `Funrec`, returning `VClosure` or `VRecClosure`).
* **Cell Referencing:** The language includes a `Cell(row, col)` expression. When the evaluator encounters this, it pauses the current context, fetches the expression located at those coordinates in the global spreadsheet grid, and evaluates it.
* **Cycle Detection:** To handle circular references (e.g., cell A references cell B, which references cell A), the engine implements a depth-based cycle detection algorithm (`is_cyclic_cell`). If the resolution depth exceeds the total number of cells in the grid (`spreadsheet_size`), a cycle is mathematically guaranteed, and the system flags it.
* **Safe Evaluation:** The main entry point (`eval_spreadsheet`) acts as a safeguard. It first runs the cycle detection over the entire grid. If a circular dependency is found, it safely returns `None`. If the grid is acyclic, it maps the evaluator over the 2D list, reducing all AST expressions into a final 2D list of evaluated `value` types.

## Scapegoat Tree Implementation (scapegoat_tree.ml)

**Problem Summary:** The project requires the implementation of a self-balancing Binary Search Tree (BST) without the overhead of storing extra balancing information (like colors in Red-Black Trees or heights in AVL Trees) in every node. The tree must dynamically adjust itself during insertions and deletions to guarantee $O(\log n)$ amortized time complexity for basic operations.

**Solution:**
The solution is implemented in OCaml as a Scapegoat Tree, a type of un-balanced BST that achieves balance by occasionally rebuilding entire subtrees.
* **Core Data Structure:** The tree is defined using a standard algebraic data type (`tree`). The `sgtree` record acts as a wrapper, keeping track of the current `size` and the historical `max_size` (necessary for triggering rebalances on deletion).
* **Alpha-Weight Balance:** The tree maintains an $\alpha$-weight balance (configured here with `alpha_num = 3` and `alpha_denom = 4`, meaning $\alpha = 0.75$). A node violates this balance if the size of either of its children exceeds $0.75$ of its own size.
* **Insertion and Scapegoat Discovery:** During insertion, the tree tracks the depth of the newly added node. If the depth exceeds the permissible limit (`alpha_height`), the algorithm walks back up the insertion path to find the "scapegoat" — the highest node that violates the $\alpha$-weight balance condition.
* **Subtree Rebuilding:** Once a scapegoat is identified, or when a deletion causes the total tree size to drop significantly relative to `max_size`, the `rebuild_balanced` function is called. It flattens the unbalanced subtree into a sorted list (`tree_to_list`) and perfectly reconstructs it by recursively picking the exact middle element (`split_list_at_middle`) as the new root.
