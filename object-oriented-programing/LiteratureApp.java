import java.io.*;
import java.util.*;

// TASK (C): Required Serializable interface for object save/load
interface Observer extends Serializable {
    void receiveNotification(Book book);
    String getIdentifier(); // Required for additional sorting by attribute
}

// TASK (B): Comparator class satisfying the Open-Closed Principle (OCP)
class ObserverComparator implements Comparator<Observer>, Serializable {
    private final List<Class<?>> typeOrder;

    public ObserverComparator(List<Class<?>> typeOrder) {
        this.typeOrder = typeOrder;
    }

    @Override
    public int compare(Observer o1, Observer o2) {
        int p1 = typeOrder.indexOf(o1.getClass());
        int p2 = typeOrder.indexOf(o2.getClass());

        // If the class is not on the priority list, move it to the end
        if (p1 == -1) p1 = Integer.MAX_VALUE;
        if (p2 == -1) p2 = Integer.MAX_VALUE;

        // Primary condition: sorting by defined class types (e.g., Critics -> Publishers)
        if (p1 != p2) {
            return Integer.compare(p1, p2);
        }
        
        // Secondary condition: Sorting by attribute (e.g., name) for objects of the same type
        return o1.getIdentifier().compareTo(o2.getIdentifier());
    }
}

class Writer implements Serializable {
    ArrayList<Book> books = new ArrayList<Book>();
    String penName;
    int birthYear; // TASK (A): Additional field of different type (int vs String)
    ArrayList<Observer> observers = new ArrayList<Observer>();

    // TASK (B): Polymorphic comparator allowing easy change without modifying this class
    private Comparator<Observer> comparator;

    Writer(String penName, int birthYear) {
        this.penName = penName;
        this.birthYear = birthYear;
        // Default order set externally
        this.comparator = new ObserverComparator(
            Arrays.asList(Critic.class, Publisher.class, Reader.class)
        );
    }

    // Method allowing easy replacement of the notification strategy
    void setNotificationOrder(Comparator<Observer> newComparator) {
        this.comparator = newComparator;
    }

    void addObserver(Observer observer) {
        if (!this.observers.contains(observer)) {
            this.observers.add(observer);
        }
    }

    void removeObserver(Observer observer) {
        this.observers.remove(observer);
    }

    void notifyObservers(Book book) {
        // TASK (B): Sorting observers taking into account the order defined in OCP
        ArrayList<Observer> sortedObservers = new ArrayList<>(this.observers);
        sortedObservers.sort(comparator);

        for (Observer observer : sortedObservers) {
            observer.receiveNotification(book);
        }
    }

    Book writeBook(String title, int publicationYear, Writer... coAuthors) {
        ArrayList<Writer> authors = new ArrayList<Writer>();
        authors.add(this);

        for (Writer author : coAuthors) {
            if (!authors.contains(author)) {
                authors.add(author);
            }
        }

        Book book = new Book(title, publicationYear, authors);

        for (Writer author : authors) {
            author.books.add(book);
        }

        for (Writer author : authors) {
            author.notifyObservers(book);
        }

        return book;
    }

    @Override
    public String toString() {
        return "Writer{penName='" + penName + "', birthYear=" + birthYear +
               ", bookCount=" + books.size() + ", observerCount=" + observers.size() + "}";
    }
}

class Book implements Serializable {
    String title;
    int publicationYear; // TASK (A): Additional field of a different type
    ArrayList<Writer> authors;

    Book(String title, int publicationYear, ArrayList<Writer> authors) {
        this.title = title;
        this.publicationYear = publicationYear;
        this.authors = new ArrayList<Writer>(authors);
    }

    String authorsAsString() {
        String result = "";
        for (int i = 0; i < authors.size(); i++) {
            result += authors.get(i).penName;
            if (i < authors.size() - 1) {
                result += ", ";
            }
        }
        return result;
    }

    @Override
    public String toString() {
        return "Book{title='" + title + "', publicationYear=" + publicationYear + ", authors=[" + authorsAsString() + "]}";
    }
}

class Publisher implements Observer {
    String name; 
    int foundingYear; // TASK (A)
    ArrayList<Book> publishedBooks = new ArrayList<Book>();

    Publisher(String name, int foundingYear) {
        this.name = name;
        this.foundingYear = foundingYear;
    }

    void publishBook(Book book) {
        System.out.println("Publishing book: " + book.title + " [publisher " + name + "]");
    }

    @Override
    public void receiveNotification(Book book) {
        if (book.title.startsWith(this.name.substring(0, 1)) && !this.publishedBooks.contains(book)) {
            this.publishedBooks.add(book);
            this.publishBook(book);
        }
    }

    @Override
    public String getIdentifier() {
        return name; // Used for sub-sorting by name
    }

    @Override
    public String toString() {
        return "Publisher{name='" + name + "', foundingYear=" + foundingYear + "}";
    }
}

class Critic implements Observer {
    String name;
    int yearsOfExperience; // TASK (A)
    ArrayList<Book> reviewedBooks = new ArrayList<Book>();

    Critic(String name, int yearsOfExperience) {
        this.name = name;
        this.yearsOfExperience = yearsOfExperience;
    }

    @Override
    public void receiveNotification(Book book) {
        if (!this.reviewedBooks.contains(book)) {
            this.reviewedBooks.add(book);
            System.out.println("Critic " + name + " is reviewing the book: " + book.title);
        }
    }

    @Override
    public String getIdentifier() {
        return name;
    }

    @Override
    public String toString() {
        return "Critic{name='" + name + "', reviewCount=" + reviewedBooks.size() + "}";
    }
}

class Reader implements Observer {
    String name;
    int age; // TASK (A)
    ArrayList<Book> readingList = new ArrayList<Book>();

    Reader(String name, int age) {
        this.name = name;
        this.age = age;
    }

    @Override
    public void receiveNotification(Book book) {
        if (!this.readingList.contains(book)) {
            this.readingList.add(book);
            System.out.println("Reader " + name + " adds book to reading list: " + book.title);
        }
    }

    @Override
    public String getIdentifier() {
        return name;
    }

    @Override
    public String toString() {
        return "Reader{name='" + name + "', age=" + age + ", readingListSize=" + readingList.size() + "}";
    }
}

// TASK (C): Wrapper grouping whole collections of objects for mass save/load.
class Database implements Serializable {
    ArrayList<Writer> writers = new ArrayList<Writer>();
    ArrayList<Book> books = new ArrayList<Book>();
    // Selected 1 observer type to verify saving
    ArrayList<Reader> readers = new ArrayList<Reader>();
}

public class LiteratureApp {
    
    // TASK (C): Application startup scenario
    public static void main(String[] args) {
        // Ability to provide a custom filename via command line
        String fileName = args.length > 0 ? args[0] : "literature_db.dat";
        File file = new File(fileName);
        Database db = null;

        // If the data file exists -> Read
        if (file.exists()) {
            System.out.println(">>> Save file found. Loading data from: " + fileName);
            try (ObjectInputStream ois = new ObjectInputStream(new FileInputStream(file))) {
                db = (Database) ois.readObject();
                System.out.println(">>> Data deserialized successfully!\n");
            } catch (IOException | ClassNotFoundException e) {
                System.err.println("Error reading file: " + e.getMessage());
            }
        } 
        // If it doesn't exist -> Generate sample data and save
        else {
            System.out.println(">>> File does not exist. Generating new data...");
            db = generateData();
            
            try (ObjectOutputStream oos = new ObjectOutputStream(new FileOutputStream(file))) {
                oos.writeObject(db);
                System.out.println("\n>>> Data has been generated and saved to file: " + fileName + "\n");
            } catch (IOException e) {
                System.err.println("Error saving file: " + e.getMessage());
            }
        }

        // Print program state (to confirm loading worked)
        if (db != null) {
            System.out.println("--- DATABASE STATE ---");
            for (Writer w : db.writers) System.out.println(w);
            for (Book b : db.books) System.out.println(b);
            System.out.println("----------------------\n");
            
            // Test action on loaded data (if loaded, check if notifications still work)
            if (!db.writers.isEmpty()) {
                System.out.println("--- Test: New book written after loading from file ---");
                Writer w = db.writers.get(0);
                Book b = w.writeBook("Astronauts", 1951);
                db.books.add(b);
            }
        }
    }

    private static Database generateData() {
        Database db = new Database();

        Writer lem = new Writer("Lem", 1921);
        Publisher publisherS = new Publisher("SuperNova", 1990);
        Critic critic1 = new Critic("Newman", 5);
        Reader reader1 = new Reader("Alice", 22);

        lem.addObserver(publisherS);
        lem.addObserver(critic1);
        lem.addObserver(reader1);

        System.out.println("--- CREATING BOOK: SOLARIS ---");
        // Logs from this write will show the new order: Newman (Critic) -> SuperNova (Publisher) -> Alice (Reader)
        Book solaris = lem.writeBook("Solaris", 1961);
        System.out.println();

        Writer aho = new Writer("Aho", 1941);
        Writer hopcroft = new Writer("Hopcroft", 1939);
        Writer ullman = new Writer("Ullman", 1942);

        Publisher publisherA = new Publisher("Academic", 1950);
        Critic critic2 = new Critic("Smith", 8);
        Reader reader2 = new Reader("Bob", 24);

        aho.addObserver(publisherA);
        aho.addObserver(critic2);
        hopcroft.addObserver(reader2);
        ullman.addObserver(critic1);

        System.out.println("--- CREATING BOOK: AiSD ---");
        Book aisd = aho.writeBook("AiSD", 1974, hopcroft, ullman);

        db.writers.addAll(Arrays.asList(lem, aho, hopcroft, ullman));
        db.books.addAll(Arrays.asList(solaris, aisd));
        db.readers.addAll(Arrays.asList(reader1, reader2));

        return db;
    }
}
