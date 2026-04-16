import javax.swing.*;
import java.awt.*;
import java.io.*;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;

class DuplicateObserverException extends Exception {
    DuplicateObserverException(String message) { super(message); }
}

class ObserverComparator implements Comparator<Observer>, Serializable {
    private static final long serialVersionUID = 1L;
    @Override
    public int compare(Observer o1, Observer o2) {
        if (o1.getPriority() != o2.getPriority()) {
            return Integer.compare(o1.getPriority(), o2.getPriority());
        }
        return o1.getId().compareTo(o2.getId());
    }
}

interface Observer extends Serializable {
    void notify(Book book);
    int getPriority();
    String getId();
}

class Author implements Serializable {
    private static final long serialVersionUID = 1L;
    private String penName;
    private int birthYear; 
    
    private ArrayList<Book> books = new ArrayList<>();
    private ArrayList<Observer> observers = new ArrayList<>();

    public Author(String penName, int birthYear) {
        this.penName = penName;
        this.birthYear = birthYear;
    }

    public String getPenName() { return penName; }
    public void setPenName(String p) { this.penName = p; }
    public int getBirthYear() { return birthYear; }
    public void setBirthYear(int r) { this.birthYear = r; }

    public void addObserver(Observer observer) throws DuplicateObserverException {
        if (this.observers.contains(observer)) {
            throw new DuplicateObserverException("Error: " + observer.getId() + " is already observing " + this.penName);
        }
        this.observers.add(observer);
    }

    public void notifyObservers(Book book) {
        Collections.sort(this.observers, new ObserverComparator());
        for (Observer observer : this.observers) {
            observer.notify(book);
        }
    }

    public Book writeBook(String title, int publishYear, Author... coAuthors) {
        ArrayList<Author> authors = new ArrayList<>();
        authors.add(this);
        for (Author author : coAuthors) {
            if (!authors.contains(author)) authors.add(author);
        }
        Book book = new Book(title, publishYear, authors);
        for (Author author : authors) {
            author.books.add(book);
            author.notifyObservers(book);
        }
        return book;
    }

    @Override
    public String toString() { return penName + " (" + birthYear + ")"; }
}

class Book implements Serializable {
    private static final long serialVersionUID = 1L;
    private String title;
    private int publishYear;
    private ArrayList<Author> authors;

    public Book(String title, int publishYear, ArrayList<Author> authors) {
        this.title = title;
        this.publishYear = publishYear;
        this.authors = new ArrayList<>(authors);
    }

    public String getTitle() { return title; }
    public void setTitle(String t) { this.title = t; }
    public int getPublishYear() { return publishYear; }
    public void setPublishYear(int r) { this.publishYear = r; }
    
    @Override
    public String toString() { return title + " [" + publishYear + "]"; }
}

class Publisher implements Observer {
    private static final long serialVersionUID = 1L;
    private char name;
    private String city;
    private ArrayList<Book> publishedBooks = new ArrayList<>();

    public Publisher(char name, String city) {
        this.name = name;
        this.city = city;
    }

    public char getName() { return name; }
    public void setName(char n) { this.name = n; }
    public String getCity() { return city; }
    public void setCity(String m) { this.city = m; }

    @Override public int getPriority() { return 2; }
    @Override public String getId() { return "Publisher " + name; }
    @Override public void notify(Book book) { }
    @Override public String toString() { return "Publisher " + name + " from " + city; }
}

class Critic implements Observer {
    private static final long serialVersionUID = 1L;
    String name;
    int experienceYears;
    public Critic(String name, int experienceYears) { this.name = name; this.experienceYears = experienceYears; }
    @Override public int getPriority() { return 1; } 
    @Override public String getId() { return name; }
    @Override public void notify(Book book) {}
}

class Reader implements Observer {
    private static final long serialVersionUID = 1L;
    String name;
    boolean premiumAccount;
    public Reader(String name, boolean premium) { this.name = name; this.premiumAccount = premium; }
    @Override public int getPriority() { return 3; }
    @Override public String getId() { return name; }
    @Override public void notify(Book book) {}
}

class Database implements Serializable {
    private static final long serialVersionUID = 1L;
    ArrayList<Author> authors = new ArrayList<>();
    ArrayList<Book> books = new ArrayList<>();
    ArrayList<Publisher> publishers = new ArrayList<>();
    ArrayList<Critic> critics = new ArrayList<>();
    ArrayList<Reader> readers = new ArrayList<>();

    public void registerAuthor(Author p) { if (!authors.contains(p)) authors.add(p); }
    public void registerBook(Book k) { if (!books.contains(k)) books.add(k); }
    public void registerPublisher(Publisher w) { if (!publishers.contains(w)) publishers.add(w); }
}

abstract class EditPanel<T> extends JPanel {
    public abstract void populateFields(T object);
    public abstract void saveChanges(T object);
}

class AuthorEditPanel extends EditPanel<Author> {
    private JTextField fieldPenName = new JTextField(20);
    private JTextField fieldYear = new JTextField(10);

    public AuthorEditPanel() {
        setLayout(new GridLayout(2, 2, 5, 5));
        add(new JLabel(" Pen Name:")); add(fieldPenName);
        add(new JLabel(" Birth Year:")); add(fieldYear);
    }

    @Override
    public void populateFields(Author p) {
        fieldPenName.setText(p.getPenName());
        fieldYear.setText(String.valueOf(p.getBirthYear()));
    }

    @Override
    public void saveChanges(Author p) {
        p.setPenName(fieldPenName.getText());
        try {
            p.setBirthYear(Integer.parseInt(fieldYear.getText()));
        } catch (NumberFormatException e) {
            JOptionPane.showMessageDialog(this, "Error: Year must be a number!", "Error", JOptionPane.ERROR_MESSAGE);
        }
    }
}

class BookEditPanel extends EditPanel<Book> {
    private JTextField fieldTitle = new JTextField(20);
    private JTextField fieldYear = new JTextField(10);

    public BookEditPanel() {
        setLayout(new GridLayout(2, 2, 5, 5));
        add(new JLabel(" Title:")); add(fieldTitle);
        add(new JLabel(" Publish Year:")); add(fieldYear);
    }

    @Override
    public void populateFields(Book k) {
        fieldTitle.setText(k.getTitle());
        fieldYear.setText(String.valueOf(k.getPublishYear()));
    }

    @Override
    public void saveChanges(Book k) {
        k.setTitle(fieldTitle.getText());
        try {
            k.setPublishYear(Integer.parseInt(fieldYear.getText()));
        } catch (NumberFormatException e) {
            JOptionPane.showMessageDialog(this, "Error: Year must be a number!", "Error", JOptionPane.ERROR_MESSAGE);
        }
    }
}

class PublisherEditPanel extends EditPanel<Publisher> {
    private JTextField fieldName = new JTextField(20);
    private JTextField fieldCity = new JTextField(20);

    public PublisherEditPanel() {
        setLayout(new GridLayout(2, 2, 5, 5));
        add(new JLabel(" Name (1 char only):")); add(fieldName);
        add(new JLabel(" City:")); add(fieldCity);
    }

    @Override
    public void populateFields(Publisher w) {
        fieldName.setText(String.valueOf(w.getName()));
        fieldCity.setText(w.getCity());
    }

    @Override
    public void saveChanges(Publisher w) {
        String enteredName = fieldName.getText().trim();
        
        if (enteredName.isEmpty()) {
            JOptionPane.showMessageDialog(this, "Error: Name cannot be empty!", "Error", JOptionPane.ERROR_MESSAGE);
            return;
        }

        w.setName(enteredName.charAt(0));
        w.setCity(fieldCity.getText());
    }
}

public class GuiApplication extends JFrame {
    private Database database;
    private String fileName = "literature_database.ser";
    private JComboBox<Object> selector;
    private EditPanel editPanel;
    private JButton btnSave;

    public GuiApplication(String editType, Database database) {
        this.database = database;
        setTitle("Editor - " + editType);
        setSize(400, 200);
        setDefaultCloseOperation(EXIT_ON_CLOSE);
        setLayout(new BorderLayout(10, 10));

        DefaultComboBoxModel<Object> model = new DefaultComboBoxModel<>();
        
        if (editType.equalsIgnoreCase("Author")) {
            database.authors.forEach(model::addElement);
            editPanel = new AuthorEditPanel();
        } else if (editType.equalsIgnoreCase("Book")) {
            database.books.forEach(model::addElement);
            editPanel = new BookEditPanel();
        } else if (editType.equalsIgnoreCase("Publisher")) {
            database.publishers.forEach(model::addElement);
            editPanel = new PublisherEditPanel();
        } else {
            JOptionPane.showMessageDialog(this, "Unknown edit type! Run with argument 'Author' or 'Book'.");
            System.exit(0);
        }

        selector = new JComboBox<>(model);
        add(selector, BorderLayout.NORTH);
        add(editPanel, BorderLayout.CENTER);

        selector.addActionListener(e -> {
            Object selected = selector.getSelectedItem();
            if (selected != null) editPanel.populateFields(selected);
        });

        btnSave = new JButton("Save changes");
        btnSave.addActionListener(e -> {
            Object selected = selector.getSelectedItem();
            if (selected != null) {
                editPanel.saveChanges(selected);
                saveDatabaseToFile();
                selector.repaint();
                JOptionPane.showMessageDialog(this, "Changes saved to database!");
            }
        });
        add(btnSave, BorderLayout.SOUTH);

        if (model.getSize() > 0) {
            selector.setSelectedIndex(0);
            editPanel.populateFields(model.getElementAt(0));
        }
    }

    private void saveDatabaseToFile() {
        try (ObjectOutputStream oos = new ObjectOutputStream(new FileOutputStream(fileName))) {
            oos.writeObject(database);
        } catch (IOException e) { e.printStackTrace(); }
    }

    public static void main(String[] args) {
        String type = (args.length > 0) ? args[0] : "Author";
        String fileName = "literature_database.ser";
        File f = new File(fileName);
        Database database = new Database();

        if (f.exists()) {
            System.out.println("Loading existing database from disk...");
            try (ObjectInputStream ois = new ObjectInputStream(new FileInputStream(f))) {
                database = (Database) ois.readObject();
            } catch (Exception e) { e.printStackTrace(); }
        } else {
            System.out.println("No file found. Generating new data...");
            Author lem = new Author("Stanislaw Lem", 1921);
            Author tolkien = new Author("J.R.R. Tolkien", 1892);
            Book solaris = lem.writeBook("Solaris", 1961);
            
            database.registerAuthor(lem);
            database.registerAuthor(tolkien);
            database.registerBook(solaris);
        }

        final Database finalDatabase = database;
        SwingUtilities.invokeLater(() -> new GuiApplication(type, finalDatabase).setVisible(true));
    }
}
