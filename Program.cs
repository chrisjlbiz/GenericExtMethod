using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericExtMethod
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] mylist = { "3", "2x", "3s", "1", "9" };

            // Custom WhereSelect
            var result1 = mylist.WhereSelect((string s, out int i) => int.TryParse(s, out i))
                                .OrderBy(i => i);

            // Regular Where and Select
            var result2 = mylist.Where(s =>
            {
                int i = 0;
                return int.TryParse(s, out i);
            })
                                .Select(s => int.Parse(s))
                                .OrderBy(i => i);

            Console.WriteLine("Custom WhereSelect method :");
            foreach (int item in result1)
                Console.WriteLine(item.ToString());


            Console.WriteLine("Regular Where and Select :");
            foreach (int item in result2)
                Console.WriteLine(item.ToString());

            Console.ReadKey(true);
        }
    }

    public delegate X CustomFunc<T, U, X>(T input, out U output);

    public static class Helper
    {
        public static IEnumerable<TResult> WhereSelect<T, TResult>(this IEnumerable<T> collection, CustomFunc<T, TResult, bool> predicate)
        {
            TResult result = default(TResult);

            foreach (T item in collection)
            {
                if (predicate(item, out result))
                    yield return result;
            }
        }
    }
}
