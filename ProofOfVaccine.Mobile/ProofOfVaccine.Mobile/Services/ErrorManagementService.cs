using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProofOfVaccine.Mobile.Services
{
    public interface IErrorManagementService
    {
        void HandleError(Exception ex);
    }

    public class ErrorManagementService : IErrorManagementService
    {

        public ErrorManagementService()
        {

        }

        public void HandleError(Exception ex)
        {
            
        }

        private void Report(Exception ex)
        {

        }

        private void InternalReport(Exception ex)
        {
           
        }
    }
}
