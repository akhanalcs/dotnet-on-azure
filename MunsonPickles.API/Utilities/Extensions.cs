namespace MunsonPickles.API.Utilities;

public static class Extensions
{
    // Fastest way of reversing a string with least allocation using SPAN
    public static string ReverseString(this string input)
    {
        // input -> state
        // Essentially input string is copied into a new character span, the span is reversed, and using that Span
        // we create a new string using string.Create
        return string.Create(input.Length, input, (chars, state) =>
        {
            state.CopyTo(chars);
            chars.Reverse();
        });
    }
}