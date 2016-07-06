using SafetySharp.Modeling;

namespace ffb.Modelling
{
    public class RadioModule : Component
    {
        public extern Request RequestIn { get; }

        public virtual Request RequestOut { get; private set; }

        public extern Response ResponseIn { get; }

        public virtual Response ResponseOut { get; protected set; }

        public override void Update()
        {
            RequestOut = RequestIn;
            ResponseOut = ResponseIn;
        }
    }
}
