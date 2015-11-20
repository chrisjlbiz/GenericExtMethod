# Merging generic extension methods – Where + Select

When working with IEnumerable collection, two of the extension methods in Linq that we often use are the Where() and Select() methods. Usually when applying both methods, rather than running them separately, we would chained them together and with other methods using the dot-notation syntax. This results in a fluent interface and also optimizes the iteration of the collection, due to the fact that both methods implements deferred execution.

Obviously we understand each method performs a specialized task; the Where method filters a sequence, while the Select method projects each element of a sequence into a new form. Assuming we use method chaining, then our choice is to run them in isolation, one after another. In a certain condition however, having the filtering and projecting tasks isolated might not seem like a good choice. Let me show an example.

Suppose we have a collection,
```c#
string[] mylist = { "3", "a", "b", "1", "9" };
```
Our aim is to transform the array elements (string) into numbers, while removing any non-numeric elements in the process. Usually we would apply method chaining using Where and Select methods.
```c#
var result = mylist.Where(s => {
                                  int i = 0; 
                                  return int.TryParse(s, out i);
                               })
                   .Select(s => int.Parse(s))
                   .OrderBy(i => i);
 
foreach (int item in result1)
    Console.WriteLine(item.ToString());
```
The output, as we expected is a sequence of number in an ascending order:
```
1
3
9
```
*Fig.1*

If you look closely to the int.TryParse() method you will notice that we cannot utilize the int i value, even though that is the value we want. Instead, we have to run another int.Parse() method inside the Select method to get the result. Such waste may be trivial, but it’s probably better if we can find a workaround to overcome the limitations imposed by the methods designs.

##GenericExtMethod

You may wonder, how about creating our own generic extension method that implements the functions of both the Where and Select methods? Seems like a very good idea.

First, lets start by declaring a custom generic delegate type that will be used as a parameter for our extension method.
```c#
delegate TResult CustomFunc<T, U, TResult>(T input, out U output)
```
The delegate signature enforces the usage of the out parameter of the lambda expression that we want to assign to the delegate instance. This allows our extension method to receive the argument value provided by the lambda via the out parameter, aside from the returned value of the lambda. From there we can code our extension method to perform an evaluation of the returned value, and return the stored argument value based on the evaluation.
```c#
public static IEnumerable<TResult> WhereSelect<T, TResult>(this IEnumerable<T> collection, CustomFunc<T, TResult, bool> predicate)
{
    TResult result = default(TResult);
 
    foreach (T item in collection)
    {
        if (predicate(item, out result))
            yield return result;
    }
}
```
To demonstrate how to use the custom extension method above, let’s go back to the collection example – mylist, and apply the int.TryParse method again.
```c#
var result = mylist.WhereSelect((string s, out int i) => int.TryParse(s, out i))
                   .OrderBy(i => i);
```
And when we run the code we will get the same result as in *Fig. 1*.
