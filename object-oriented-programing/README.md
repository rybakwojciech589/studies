### Object-Oriented Programming: Boolean Expression Tree (BooleanExpressionTree.cs)

**Problem Summary:** The task involves designing an object-oriented data structure to represent, evaluate, and simplify Boolean logic formulas. The structure must handle constants, variables, and standard logical operations (AND, OR, NOT) while optimizing memory usage and allowing easy traversal of the generated expression tree.
**Solution:** The solution is implemented in C# using advanced object-oriented design patterns and language mechanisms.
* **Composite Pattern:** The abstract `Formula` class serves as the base for building an Abstract Syntax Tree (AST). Logical operations (`And`, `Or`, `Not`) hold references to other formulas, creating a recursive tree structure.
* **Flyweight Pattern:** To optimize memory, constants (`true`, `false`) and variables with the same name are instantiated only once. The static `Create` factory methods manage a dictionary of existing instances, ensuring that identical variables always point to the exact same object in memory.
* **Operator Overloading:** Standard C# operators (`&`, `|`, `!`) are overloaded to allow building complex formulas naturally and intuitively (e.g., `!x | (y & z)`).
* **Expression Simplification:** A recursive `simplify()` method reduces the tree by eliminating double negations (e.g., `!!x -> x`) and resolving operations involving constants (e.g., `x & false -> false`, `x | true -> true`).
* **Iteration:** The `Formula` class implements the `IEnumerable<Formula>` interface. By utilizing `yield return`, it allows a clean Pre-Order traversal of the entire expression tree using a standard `foreach` loop.

---

### Object-Oriented Programming: Literature Observer System (LiteratureApp.java)

**Problem Summary:** The task requires building an object-oriented system modeling writers, books, and various observers (publishers, critics, readers). The system must enforce a strict, easily modifiable notification order upon new book releases and support persistent data storage across application runs.
**Solution:** The solution is implemented in Java, emphasizing the SOLID design principles and standard Java I/O for state persistence.
* **Observer Pattern:** Writers act as subjects, automatically notifying registered entities (Critics, Publishers, Readers) whenever a new book is authored.
* **Open-Closed Principle (OCP):** A custom comparator (`ObserverComparator`) dictates the notification order based on class types and specific object attributes. This design allows for injecting new notification strategies or adding entirely new observer classes without modifying the existing `Writer` class.
* **Data Serialization:** The application state (collections of writers, books, and readers) is grouped into a `Database` wrapper and serialized to a binary file (`.dat`) using Java's `Serializable` interface. The application automatically loads existing data on startup or generates mock data if the file does not exist.
* **Complex Object Associations:** The model maintains rich relationships, such as multi-author books, tracking reading lists for readers, and logging publishing history for publishers, demonstrating strong encapsulation and object interaction.
