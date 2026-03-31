## Object-Oriented Programming: Boolean Expression Tree (BooleanExpressionTree.cs)

**Problem Summary:** The task involves designing an object-oriented data structure to represent, evaluate, and simplify Boolean logic formulas. The structure must handle constants, variables, and standard logical operations (AND, OR, NOT) while optimizing memory usage and allowing easy traversal of the generated expression tree.

**Solution:**
The solution is implemented in C# using advanced object-oriented design patterns and language mechanisms.
* **Composite Pattern:** The abstract `Formula` class serves as the base for building an Abstract Syntax Tree (AST). Logical operations (`And`, `Or`, `Not`) hold references to other formulas, creating a recursive tree structure.
* **Flyweight Pattern:** To optimize memory, constants (`true`, `false`) and variables with the same name are instantiated only once. The static `Create` factory methods manage a dictionary of existing instances, ensuring that identical variables always point to the exact same object in memory.
* **Operator Overloading:** Standard C# operators (`&`, `|`, `!`) are overloaded to allow building complex formulas naturally and intuitively (e.g., `!x | (y & z)`).
* **Expression Simplification:** A recursive `simplify()` method reduces the tree by eliminating double negations (e.g., `!!x -> x`) and resolving operations involving constants (e.g., `x & false -> false`, `x | true -> true`).
* **Iteration:** The `Formula` class implements the `IEnumerable<Formula>` interface. By utilizing `yield return`, it allows a clean Pre-Order traversal of the entire expression tree using a standard `foreach` loop.
