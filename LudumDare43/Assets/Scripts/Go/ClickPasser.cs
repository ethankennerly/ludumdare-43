namespace FineGameDesign.Go
{
    public sealed class ClickPasser : ClickCollider
    {
        protected override void HandleClick()
        {
            base.HandleClick();
            Referee.instance.Pass();
        }
    }
}
