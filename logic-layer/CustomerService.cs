using data_layer;

namespace logic_layer
    {
        public class CustomerService
        {
            public CustomerRepo CustomerRepo = new CustomerRepo();

            public List<CustomerViewModel> GetCustomersByIds(int[] customerIds)
            {
                CustomerRepo.GetAllCustomersByIds(customerIds);
                return _customerRepo.CustomerDTOList
                    .Select(dto => new CustomerViewModel(dto.CustomerId, dto.Name))
                    .ToList();
        }
    }

}