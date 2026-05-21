using data_layer;

namespace logic_layer
{
    public class CustomerService
    {
        private readonly CustomerRepo CustomerRepo = new();

        public List<CustomerModel> GetAllCustomersById(List<int> customerIdList)
        {
            List<CustomerDTO> customerDTOList = CustomerRepo.GetAllCustomersByIds(customerIdList);
            List<CustomerModel> customerModelList = [];
            foreach (CustomerDTO customer in customerDTOList)
            {
                customerModelList.Add(new(customer.CustomerId, customer.Name));
            }
            return customerModelList;
        }
    }
}