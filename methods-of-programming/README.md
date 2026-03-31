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
