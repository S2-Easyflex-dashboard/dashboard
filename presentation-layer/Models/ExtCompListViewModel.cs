namespace presentation_layer.Models
{
    public class ExtCompListViewModel
    {
        public List<IpInfoViewModel> DuplicateIps { get; private set; }

        public ExtCompListViewModel(List<IpInfoViewModel> duplicateIps)
        {
            DuplicateIps = duplicateIps;
        }
    }
}
