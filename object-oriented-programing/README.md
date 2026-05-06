### Object-Oriented Programming: Boolean Expression Tree (BooleanExpressionTree.cs)

**Problem Summary:** The task involves designing an object-oriented data structure to represent, evaluate, and simplify Boolean logic formulas. The structure must handle constants, variables, and standard logical operations (AND, OR, NOT) while optimizing memory usage and allowing easy traversal of the generated expression tree.
**Solution:** The solution is implemented in C# using advanced object-oriented design patterns and language mechanisms.
* **Composite Pattern:** The abstract `Formula` class serves as the base for building an Abstract Syntax Tree (AST). Logical operations (`And`, `Or`, `Not`) hold references to other formulas, creating a recursive tree structure.
* **Flyweight Pattern:** To optimize memory, constants (`true`, `false`) and variables with the same name are instantiated only once. The static `Create` factory methods manage a dictionary of existing instances, ensuring that identical variables always point to the exact same object in memory.
* **Operator Overloading:** Standard C# operators (`&`, `|`, `!`) are overloaded to allow building complex formulas naturally and intuitively (e.g., `!x | (y & z)`).
* **Expression Simplification:** A recursive `simplify()` method reduces the tree by eliminating double negations (e.g., `!!x -> x`) and resolving operations involving constants (e.g., `x & false -> false`, `x | true -> true`).
* **Iteration:** The `Formula` class implements the `IEnumerable<Formula>` interface. By utilizing `yield return`, it allows a clean Pre-Order traversal of the entire expression tree using a standard `foreach` loop.

---

### Object-Oriented Programming: Literature Observer System (GuiApplication.java)

**Problem Summary:** The task requires building an object-oriented system modeling authors, books, and various observers (publishers, critics, readers) with a strict, priority-based notification order upon new book releases. Additionally, it requires a graphical user interface (GUI) to seamlessly edit entity properties and persistent data storage across application runs.
**Solution:** The solution is implemented in Java, emphasizing design patterns, generic programming, and Java Swing for the UI.
* **Observer Pattern & Prioritization:** Authors act as subjects, automatically notifying registered entities whenever a new book is authored. The notification order is strictly enforced by an `ObserverComparator` that sorts observers based on a predefined priority level (e.g., Critics first, then Publishers, then Readers) and their unique IDs. A custom exception (`DuplicateObserverException`) prevents duplicate observer registrations.
* **Generics and GUI (Swing):** The application features a dynamic graphical interface built with Java Swing. It utilizes an abstract, generic `EditPanel<T>` class that is concretely implemented for different entities (`AuthorEditPanel`, `BookEditPanel`, `PublisherEditPanel`). This allows for seamless selection, modification, and validation of object properties directly within the UI.
* **Data Serialization:** The entire application state is grouped into a central `Database` wrapper and serialized to a binary file (`literature_database.ser`) using Java's `Serializable` interface. The application automatically loads existing data on startup or generates initial mock data (e.g., Stanislaw Lem, J.R.R. Tolkien) if the file does not exist.
* **Complex Object Associations:** The model maintains rich relationships, such as establishing multi-author books dynamically, preventing duplicate author entries, and safely updating the centralized database state directly through interactions with the Swing UI components.


### Object-Oriented Programming: Card War Game with Template Method (card_war_template_method.rb)

**Problem Summary:** The task involves extending an object-oriented implementation of a two-player card game by identifying a reusable gameplay scheme and separating it from the concrete game logic. The program models the classic card game "War", supports a human player versus a computer opponent, and allows the game state to be saved and restored between runs.

**Solution:** The solution is implemented in Ruby and focuses on code reuse, object-oriented design, and persistence.

* **Template Method Pattern:** The abstract `GraDwuosobowa` class defines the general structure of a two-player game in the `graj` method. It controls the main game loop, checks whether the game has ended, executes turns, displays the result, and handles saving. The concrete `Gra` class inherits from it and provides the specific logic for the card game War by implementing methods such as `koniec?`, `tura`, `wyswietl_wynik`, and `obsluz_zapis`.

* **Reusable Game Loop:** The main gameplay algorithm is separated from the rules of the specific game. This makes the structure reusable for other two-player games, where only the details of a turn, ending condition, and result display would need to be changed.

* **Card Representation and Comparison:** The `Karta` class represents a single playing card with a figure, suit, and numeric value. It includes Ruby's `Comparable` module and defines the `<=>` operator, allowing cards to be compared naturally using operators such as `>`, `<`, and `==`.

* **Flyweight-like Card Storage:** Cards are created through the `Karta.pobierz` factory method, which stores already created card objects in a class-level hash. This ensures that the same card is not unnecessarily instantiated multiple times.

* **Human and Computer Players:** The program defines separate classes for a human player and a computer player. Both players store their cards, can receive new cards, check whether they still have cards, and play the next card. The human player can additionally interrupt the game by typing `z`.

* **Game State Serialization:** The `Zapisywalny` module adds the ability to save and load the complete game state using Ruby's `Marshal` mechanism. When the user chooses to stop the game, the current state is written to `stan_gry.dat`, and it can be restored when the program is launched again.

* **War Game Logic:** During each turn, both players play one card. The player with the stronger card takes all cards from the table. In case of a tie, the game enters a "war" state, where additional cards are placed on the table until the tie is resolved.
