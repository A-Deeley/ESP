namespace Backend;

public interface IPageViewModel
{
    event EventHandler<SGIEventArgs> ViewChanged;
    string PageId { get; set; }
    string Title { get; set; }
}
