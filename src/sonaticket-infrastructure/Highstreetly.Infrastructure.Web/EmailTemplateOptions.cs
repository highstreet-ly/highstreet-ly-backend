namespace Highstreetly.Infrastructure
{
    public class EmailTemplateOptions
    {
        public string ForgotPassword { get; set; }
        public string PasswordReset { get; set; }
        public string MagicLink { get; set; }
        public string Registration { get; set; }
        public string OrderInTheBag { get; set; }
        public string OrderInTheBagOperator { get; set; }
        public string OrderRefunded { get; set; }
        public string OrderProcessingComplete { get; set; }
        public string OrderProcessing { get; set; }
    }
}
