namespace MyLogger
{
    public class Flags<T>
    {
        public T Info { get; set; }

        public T Warning { get; set; }

        public T Error { get; set; }

        public T Exception { get; set; }

        public T Debug { get; set; }
    }
}
