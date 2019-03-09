namespace PubSub.Messages
{
    public class TestMessage
    {
        public string Text { get; }
        public string Text2 { get; }

        public TestMessage(string text, string text2)
        {
            Text = text;
            Text2 = text2;
        }
    }
}
