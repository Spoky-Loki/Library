using Library.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library.Data.Interfaces
{
    interface ILoanRepository
    {
        Task<List<LoanIndexViewModel>> GetLoans();

        LoanCreateViewModel GetEmptyLoan();

        Task AddLoan(LoanCreateViewModel loanModel);

        Task SetReturnDate(int id);

        Task<int> GetBookId(int id);

        Task<bool> IsReturned(int id);

        Task<List<LoanIndexViewModel>> GetReturned();
    }
}
