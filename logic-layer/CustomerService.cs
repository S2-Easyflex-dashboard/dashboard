using data_layer;

namespace logic_layer
{
    public class CustomerService
    {
        private CustomerRepo CustomerRepo = new CustomerRepo();
        public List<CustomerModel> CustomerModelList { get; private set; } = [];

        public CustomerService(List<int> customerId)
        {
            CustomerRepo.GetAllCustomersByIds(customerId);
            foreach (CustomerDTO customer in CustomerRepo.CustomerDTOList)
            {
                CustomerModelList.Add(new CustomerModel(customer.CustomerId, customer.Name));
            }
        }

        //public List<string> GetCustomerNamesByIds(int[] customerIds)
        //{
        //    List<string> customerNames = [];
        //    foreach (CustomerModel customer in CustomerModelList)
        //    {
        //        if (customerIds.Contains(customer.CustomerId))
        //        {
        //            customerNames.Add(customer.Name);
        //        }
        //    }
        //    return customerNames;
        //}
    }
}