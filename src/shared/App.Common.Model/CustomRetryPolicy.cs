namespace App.Common.Domain
{
    public class CustomRetryPolicy
    {
        public int MaxRetryCount { get; set; }
        public int DelaySeconds { get; set; }
    }
}
