using data_layer;

namespace logic_layer
{
    public class CustomerService
    {
        private CustomerRepo _customerRepo = new CustomerRepo();

        public List<CustomerViewModel> GetCustomersByIds(int[] customerIds)
        {
            _customerRepo.GetAllCustomersByIds(customerIds);
            return _customerRepo.CustomerDTOList
                .Select(dto => new CustomerViewModel(dto.CustomerId, dto.Name))
                .ToList();
        }

        public string GetCustomerNameById(int customerId)
        {
            _customerRepo.GetAllCustomersByIds([customerId]);
            var customer = _customerRepo.CustomerDTOList.FirstOrDefault();
            return customer != null ? customer.Name : customerId.ToString();
        }

        public List<string> GetCustomerNamesByIds(int[] customerIds)
        {
            _customerRepo.GetAllCustomersByIds(customerIds);
            return customerIds
                .Select(id =>
                {
                    var customer = _customerRepo.CustomerDTOList.FirstOrDefault(c => c.CustomerId == id);
                    return customer != null ? customer.Name : id.ToString();
                })
                .ToList();
        }
    }
}