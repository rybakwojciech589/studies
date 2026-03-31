using System;
using System.Collections;
using System.Collections.Generic;

namespace OOPZadanie3
{
    public abstract class Formula : IEnumerable<Formula>
    {
        public abstract bool evaluate(Dictionary<string, bool> s);

        public abstract Formula simplify();

        public abstract override string ToString();

        public abstract override bool Equals(object obj);

        public abstract override int GetHashCode();

        public static Formula operator !(Formula f)
        {
            return new Not(f);
        }

        public static Formula operator &(Formula a, Formula b)
        {
            return new And(a, b);
        }

        public static Formula operator |(Formula a, Formula b)
        {
            return new Or(a, b);
        }

        public abstract IEnumerator<Formula> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class Stala : Formula
    {
        public bool Value { get; }

        private static readonly Stala TrueInstance = new Stala(true);
        private static readonly Stala FalseInstance = new Stala(false);

        private Stala(bool v)
        {
            this.Value = v;
        }

        public static Stala Create(bool v)
        {
            return v ? TrueInstance : FalseInstance;
        }

        public override bool evaluate(Dictionary<string, bool> s)
        {
            return Value;
        }

        public override Formula simplify()
        {
            return this;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
                return false;

            Stala other = (Stala)obj;
            return this.Value == other.Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override IEnumerator<Formula> GetEnumerator()
        {
            yield return this;
        }
    }

    public class Zmienna : Formula
    {
        public string Name { get; }

        private static readonly Dictionary<string, Zmienna> instances =
            new Dictionary<string, Zmienna>();

        private Zmienna(string n)
        {
            this.Name = n;
        }

        public static Zmienna Create(string n)
        {
            if (!instances.ContainsKey(n))
            {
                instances[n] = new Zmienna(n);
            }

            return instances[n];
        }

        public override bool evaluate(Dictionary<string, bool> s)
        {
            if (s.ContainsKey(Name))
            {
                return s[Name];
            }
            throw new Exception($"Nieznana zmienna: {Name}");
        }

        public override Formula simplify()
        {
            return this;
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
                return false;

            Zmienna other = (Zmienna)obj;
            return this.Name == other.Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override IEnumerator<Formula> GetEnumerator()
        {
            yield return this;
        }
    }

    public class Not : Formula
    {
        public Formula SubFormula { get; }

        public Not(Formula f)
        {
            this.SubFormula = f;
        }

        public override bool evaluate(Dictionary<string, bool> s)
        {
            return !SubFormula.evaluate(s);
        }

        public override Formula simplify()
        {
            Formula simplifiedSub = SubFormula.simplify();
            if (simplifiedSub is Stala stala)
            {
                return Stala.Create(!stala.Value);
            }
            if (simplifiedSub is Not not)
            {
                return not.SubFormula;
            }
            return new Not(simplifiedSub);
        }

        public override string ToString()
        {
            return $"~({SubFormula})";
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
                return false;

            Not other = (Not)obj;
            return this.SubFormula.Equals(other.SubFormula);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SubFormula);
        }

        public override IEnumerator<Formula> GetEnumerator()
        {
            yield return this;

            foreach (Formula f in SubFormula)
            {
                yield return f;
            }
        }
    }

    public class And : Formula
    {
        private Formula left;
        private Formula right;

        public And(Formula l, Formula r)
        {
            this.left = l;
            this.right = r;
        }

        public override bool evaluate(Dictionary<string, bool> s)
        {
            return left.evaluate(s) && right.evaluate(s);
        }

        public override Formula simplify()
        {
            Formula newleft = left.simplify();
            Formula newright = right.simplify();

            if (newleft is Stala stalaLeft)
            {
                if (!stalaLeft.Value) return Stala.Create(false);
                return newright;
            }

            if (newright is Stala stalaRight)
            {
                if (!stalaRight.Value) return Stala.Create(false);
                return newleft;
            }

            return new And(newleft, newright);
        }

        public override string ToString()
        {
            return $"({left} & {right})";
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
                return false;

            And other = (And)obj;
            return this.left.Equals(other.left) && this.right.Equals(other.right);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(left, right);
        }

        public override IEnumerator<Formula> GetEnumerator()
        {
            yield return this;

            foreach (Formula f in left)
            {
                yield return f;
            }

            foreach (Formula f in right)
            {
                yield return f;
            }
        }
    }

    public class Or : Formula
    {
        private Formula left;
        private Formula right;

        public Or(Formula l, Formula r)
        {
            this.left = l;
            this.right = r;
        }

        public override bool evaluate(Dictionary<string, bool> s)
        {
            return left.evaluate(s) || right.evaluate(s);
        }

        public override Formula simplify()
        {
            Formula newleft = left.simplify();
            Formula newright = right.simplify();

            if (newleft is Stala stalaLeft)
            {
                if (stalaLeft.Value) return Stala.Create(true);
                return newright;
            }

            if (newright is Stala stalaRight)
            {
                if (stalaRight.Value) return Stala.Create(true);
                return newleft;
            }

            return new Or(newleft, newright);
        }

        public override string ToString()
        {
            return $"({left} | {right})";
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
                return false;

            Or other = (Or)obj;
            return this.left.Equals(other.left) && this.right.Equals(other.right);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(left, right);
        }

        public override IEnumerator<Formula> GetEnumerator()
        {
            yield return this;

            foreach (Formula f in left)
            {
                yield return f;
            }

            foreach (Formula f in right)
            {
                yield return f;
            }
        }
    }

    public class TestyZadanie3
    {
        public static void Uruchom()
        {
            Console.WriteLine("--- Uruchamiam Testy Zadanie 3 ---\n");

            var s = new Dictionary<string, bool>
            {
                { "x", true },
                { "y", false },
                { "z", true }
            };

            Console.WriteLine("=== TESTY EVALUATE ===");
            Console.WriteLine($"Stan zmiennych: x={s["x"]}, y={s["y"]}, z={s["z"]}\n");

            Formula e1 = !Zmienna.Create("x") | (Zmienna.Create("y") & Stala.Create(true));

            Console.WriteLine($"Wyrażenie: {e1}");
            Console.WriteLine($"Wynik:     {e1.evaluate(s)}\n");

            Formula e2 =
                !(Zmienna.Create("x") | !Zmienna.Create("y"))
                | (Zmienna.Create("z") & Stala.Create(false));

            Console.WriteLine($"Wyrażenie: {e2}");
            Console.WriteLine($"Wynik:     {e2.evaluate(s)}\n");


            Console.WriteLine("=== TESTY SIMPLIFY ===");

            Formula s1 =
                !!Zmienna.Create("x")
                & (Zmienna.Create("y") | Stala.Create(true));

            Console.WriteLine($"Przed uproszczeniem: {s1}");
            Console.WriteLine($"Po uproszczeniu:     {s1.simplify()}\n");

            Formula s2 =
                ((Zmienna.Create("x") & Stala.Create(false)) | Stala.Create(true))
                & Zmienna.Create("z");

            Console.WriteLine($"Przed uproszczeniem: {s2}");
            Console.WriteLine($"Po uproszczeniu:     {s2.simplify()}\n");

            Formula s3 =
                (Zmienna.Create("x") | Stala.Create(false))
                & !Stala.Create(false);

            Console.WriteLine($"Przed uproszczeniem: {s3}");
            Console.WriteLine($"Po uproszczeniu:     {s3.simplify()}\n");

            Formula s4 =
                (Zmienna.Create("x") | Stala.Create(false))
                & !!Zmienna.Create("y");

            Console.WriteLine($"Przed uproszczeniem: {s4}");
            Console.WriteLine($"Po uproszczeniu:     {s4.simplify()}\n");

            Console.WriteLine("=== TESTY EQUALS ===");

            Formula f1 = Zmienna.Create("x") & (Zmienna.Create("y") | Stala.Create(true));
            Formula f2 = Zmienna.Create("x") & (Zmienna.Create("y") | Stala.Create(true));
            Formula f3 = Zmienna.Create("x") & (Zmienna.Create("z") | Stala.Create(true));

            Console.WriteLine($"{f1} == {f2} ? {f1.Equals(f2)}");
            Console.WriteLine($"{f1} == {f3} ? {f1.Equals(f3)}");

            Formula f4 = !!Zmienna.Create("x");
            Formula f5 = !!Zmienna.Create("x");
            Formula f6 = !Zmienna.Create("x");

            Console.WriteLine($"{f4} == {f5} ? {f4.Equals(f5)}");
            Console.WriteLine($"{f4} == {f6} ? {f4.Equals(f6)}\n");

            Console.WriteLine("=== TESTY OPERATORÓW ===");

            Formula op1 = !Zmienna.Create("x");
            Console.WriteLine($"!x = {op1}");
            Console.WriteLine($"Wynik dla x=true: {op1.evaluate(s)}\n");

            Formula op2 = Zmienna.Create("x") & Zmienna.Create("z");
            Console.WriteLine($"x & z = {op2}");
            Console.WriteLine($"Wynik: {op2.evaluate(s)}\n");

            Formula op3 = Zmienna.Create("y") | Zmienna.Create("z");
            Console.WriteLine($"y | z = {op3}");
            Console.WriteLine($"Wynik: {op3.evaluate(s)}\n");

            Formula op4 = !Zmienna.Create("x") | (Zmienna.Create("y") & Stala.Create(true));
            Formula op4_konstruktor = new Or(
                new Not(Zmienna.Create("x")),
                new And(Zmienna.Create("y"), Stala.Create(true))
            );

            Console.WriteLine($"Zapis operatorowy:    {op4}");
            Console.WriteLine($"Zapis konstruktorowy: {op4_konstruktor}");
            Console.WriteLine($"Czy są równe?         {op4.Equals(op4_konstruktor)}");
            Console.WriteLine($"Czy dają ten sam wynik? {op4.evaluate(s) == op4_konstruktor.evaluate(s)}\n");

            Formula op5 = !!Zmienna.Create("y");
            Console.WriteLine($"!!y = {op5}");
            Console.WriteLine($"Wynik: {op5.evaluate(s)}");
            Console.WriteLine($"Po uproszczeniu: {op5.simplify()}\n");

            Formula op6 = (Zmienna.Create("x") | Stala.Create(false)) & !Stala.Create(false);
            Console.WriteLine($"(x | false) & !false = {op6}");
            Console.WriteLine($"Wynik: {op6.evaluate(s)}");
            Console.WriteLine($"Po uproszczeniu: {op6.simplify()}\n");

            Console.WriteLine("=== TESTY FOREACH ===");

            Formula tree = !(Zmienna.Create("x") & Zmienna.Create("y")) | Stala.Create(true);
            Console.WriteLine($"Drzewo: {tree}");
            Console.WriteLine("Preorder:");

            foreach (Formula f in tree)
            {
                Console.WriteLine($"- {f} [{f.GetType().Name}]");
            }

            Console.WriteLine();

            Console.WriteLine("=== TESTY PODPUNKTU E ===");

            Stala a = Stala.Create(false);
            Stala b = Stala.Create(false);
            Stala c = Stala.Create(true);
            Stala d = Stala.Create(true);

            Console.WriteLine($"false instance 1: {a}");
            Console.WriteLine($"false instance 2: {b}");
            Console.WriteLine($"Czy to ten sam obiekt? {Object.ReferenceEquals(a, b)}");

            Console.WriteLine($"true instance 1: {c}");
            Console.WriteLine($"true instance 2: {d}");
            Console.WriteLine($"Czy to ten sam obiekt? {Object.ReferenceEquals(c, d)}");

            Console.WriteLine($"Czy false i true to ten sam obiekt? {Object.ReferenceEquals(a, c)}\n");

            Zmienna x1 = Zmienna.Create("x");
            Zmienna x2 = Zmienna.Create("x");
            Zmienna y1 = Zmienna.Create("y");

            Console.WriteLine($"x1 = {x1}");
            Console.WriteLine($"x2 = {x2}");
            Console.WriteLine($"y1 = {y1}");
            Console.WriteLine($"Czy x1 i x2 to ten sam obiekt? {Object.ReferenceEquals(x1, x2)}");
            Console.WriteLine($"Czy x1 i y1 to ten sam obiekt? {Object.ReferenceEquals(x1, y1)}");
        }
    }
}
