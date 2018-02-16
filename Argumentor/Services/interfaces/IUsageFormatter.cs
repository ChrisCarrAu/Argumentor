namespace ArgumentRes.Services.interfaces
{
    /// <summary>
    /// Usage formatter for a type T
    /// </summary>
    /// <typeparam name="T">Type to display usage information for</typeparam>
    public interface IUsageFormatter<T>
    {
        /// <summary>
        /// Returns the formatted Usage string for the type T
        /// </summary>
        /// <returns>Usage information</returns>
        string ToString();
    }
}
