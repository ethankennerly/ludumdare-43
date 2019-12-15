using FineGameDesign.Go.AI;

namespace FineGameDesign.Go
{
    public sealed class ClickPasser : ClickCollider
    {
        protected override void HandleClick()
        {
            base.HandleClick();
            Referee.instance.Pass();
            Referee5x5.instance.Pass();
        }
    }
}
