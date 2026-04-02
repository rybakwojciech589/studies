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
            throw new Exception($"Unknown variable: {Name}");
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

    public class Task3Tests
    {
        public static void Run()
        {
            Console.WriteLine("--- Running Task 3 Tests ---\n");

            var s = new Dictionary<string, bool>
            {
                { "x", true },
                { "y", false },
                { "z", true }
            };

            Console.WriteLine("=== EVALUATE TESTS ===");
            Console.WriteLine($"Variable state: x={s["x"]}, y={s["y"]}, z={s["z"]}\n");

            Formula e1 = !Zmienna.Create("x") | (Zmienna.Create("y") & Stala.Create(true));

            Console.WriteLine($"Expression: {e1}");
            Console.WriteLine($"Result:     {e1.evaluate(s)}\n");

            Formula e2 =
                !(Zmienna.Create("x") | !Zmienna.Create("y"))
                | (Zmienna.Create("z") & Stala.Create(false));

            Console.WriteLine($"Expression: {e2}");
            Console.WriteLine($"Result:     {e2.evaluate(s)}\n");


            Console.WriteLine("=== SIMPLIFY TESTS ===");

            Formula s1 =
                !!Zmienna.Create("x")
                & (Zmienna.Create("y") | Stala.Create(true));

            Console.WriteLine($"Before simplification: {s1}");
            Console.WriteLine($"After simplification:  {s1.simplify()}\n");

            Formula s2 =
                ((Zmienna.Create("x") & Stala.Create(false)) | Stala.Create(true))
                & Zmienna.Create("z");

            Console.WriteLine($"Before simplification: {s2}");
            Console.WriteLine($"After simplification:  {s2.simplify()}\n");

            Formula s3 =
                (Zmienna.Create("x") | Stala.Create(false))
                & !Stala.Create(false);

            Console.WriteLine($"Before simplification: {s3}");
            Console.WriteLine($"After simplification:  {s3.simplify()}\n");

            Formula s4 =
                (Zmienna.Create("x") | Stala.Create(false))
                & !!Zmienna.Create("y");

            Console.WriteLine($"Before simplification: {s4}");
            Console.WriteLine($"After simplification:  {s4.simplify()}\n");

            Console.WriteLine("=== EQUALS TESTS ===");

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

            Console.WriteLine("=== OPERATOR TESTS ===");

            Formula op1 = !Zmienna.Create("x");
            Console.WriteLine($"!x = {op1}");
            Console.WriteLine($"Result for x=true: {op1.evaluate(s)}\n");

            Formula op2 = Zmienna.Create("x") & Zmienna.Create("z");
            Console.WriteLine($"x & z = {op2}");
            Console.WriteLine($"Result: {op2.evaluate(s)}\n");

            Formula op3 = Zmienna.Create("y") | Zmienna.Create("z");
            Console.WriteLine($"y | z = {op3}");
            Console.WriteLine($"Result: {op3.evaluate(s)}\n");

            Formula op4 = !Zmienna.Create("x") | (Zmienna.Create("y") & Stala.Create(true));
            Formula op4_constructor = new Or(
                new Not(Zmienna.Create("x")),
                new And(Zmienna.Create("y"), Stala.Create(true))
            );

            Console.WriteLine($"Operator syntax:    {op4}");
            Console.WriteLine($"Constructor syntax: {op4_constructor}");
            Console.WriteLine($"Are they equal?           {op4.Equals(op4_constructor)}");
            Console.WriteLine($"Do they yield same result? {op4.evaluate(s) == op4_constructor.evaluate(s)}\n");

            Formula op5 = !!Zmienna.Create("y");
            Console.WriteLine($"!!y = {op5}");
            Console.WriteLine($"Result: {op5.evaluate(s)}");
            Console.WriteLine($"After simplification: {op5.simplify()}\n");

            Formula op6 = (Zmienna.Create("x") | Stala.Create(false)) & !Stala.Create(false);
            Console.WriteLine($"(x | false) & !false = {op6}");
            Console.WriteLine($"Result: {op6.evaluate(s)}");
            Console.WriteLine($"After simplification: {op6.simplify()}\n");

            Console.WriteLine("=== FOREACH TESTS ===");

            Formula tree = !(Zmienna.Create("x") & Zmienna.Create("y")) | Stala.Create(true);
            Console.WriteLine($"Tree: {tree}");
            Console.WriteLine("Preorder:");

            foreach (Formula f in tree)
            {
                Console.WriteLine($"- {f} [{f.GetType().Name}]");
            }

            Console.WriteLine();

            Console.WriteLine("=== SUBTASK E TESTS ===");

            Stala a = Stala.Create(false);
            Stala b = Stala.Create(false);
            Stala c = Stala.Create(true);
            Stala d = Stala.Create(true);

            Console.WriteLine($"false instance 1: {a}");
            Console.WriteLine($"false instance 2: {b}");
            Console.WriteLine($"Are they the same object? {Object.ReferenceEquals(a, b)}");

            Console.WriteLine($"true instance 1: {c}");
            Console.WriteLine($"true instance 2: {d}");
            Console.WriteLine($"Are they the same object? {Object.ReferenceEquals(c, d)}");

            Console.WriteLine($"Are false and true the same object? {Object.ReferenceEquals(a, c)}\n");

            Zmienna x1 = Zmienna.Create("x");
            Zmienna x2 = Zmienna.Create("x");
            Zmienna y1 = Zmienna.Create("y");

            Console.WriteLine($"x1 = {x1}");
            Console.WriteLine($"x2 = {x2}");
            Console.WriteLine($"y1 = {y1}");
            Console.WriteLine($"Are x1 and x2 the same object? {Object.ReferenceEquals(x1, x2)}");
            Console.WriteLine($"Are x1 and y1 the same object? {Object.ReferenceEquals(x1, y1)}");
        }
    }
}
