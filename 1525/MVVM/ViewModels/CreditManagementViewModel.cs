using System.Windows.Input;
using PDTUtils.Native;

namespace PDTUtils.MVVM.ViewModels
{
    class CreditManagementViewModel : ObservableObject
    {
        public int Bank { get; set; }
        public int Credits { get; set; }
        
        public CreditManagementViewModel()
        {
            Credits = 0;
            this.GetCredits();
            this.RaisePropertyChangedEvent("Credits");
        }
        
        public ICommand GetCreditsLevel
        {
            get { return new DelegateCommand(o => GetCredits()); }
        }
        void GetCredits()
        {
            Credits = BoLib.getCredit();
        }

        public ICommand GetBankLevel
        {
            get { return new DelegateCommand(o => GetBank()); }
        }
        void GetBank()
        {
            Bank = BoLib.getBank();
        }

        public ICommand AddCreditsToBank
        {
            get { return new DelegateCommand(o => AddCredits()); }
        }
        void AddCredits()
        {
            BoLib.addCredit(1000);
        }

        public ICommand ClearCreditLevel
        {
            get { return new DelegateCommand(o => ClearCredits()); }
        }
        void ClearCredits()
        {
            BoLib.clearBankAndCredit();
        }
    }
}
