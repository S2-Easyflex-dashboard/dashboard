namespace presentation_layer.Models
{
    public class ExtCompListViewModel
    {
        public IpInfoViewModel[] DuplicateIps { get; private set; }

        public ExtCompListViewModel(IpInfoViewModel[] duplicateIps)
        {
            DuplicateIps = duplicateIps;
        }
    }
}
