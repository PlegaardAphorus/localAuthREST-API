namespace LocalAuthREST_API.models
{
    public class Todo
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public bool Completed { get; set; }

        public DateTime deadline { get; set; }
    }
}
