using CommunityToolkit.Mvvm.ComponentModel; // 确保引用了这个
// 或者使用 System.ComponentModel.INotifyPropertyChanged (如果不使用 Toolkit)

namespace AtelierWiki.Data.A11
{
    // 继承 ObservableObject 以获得 SetProperty 和 OnPropertyChanged 功能
    public class Feature : ObservableObject
    {
        private int _id;
        public int Id { get => _id; set => SetProperty(ref _id, value); }

        private string _name;
        public string Name { get => _name; set => SetProperty(ref _name, value); }

        private string _extension;
        public string Extension { get => _extension; set => SetProperty(ref _extension, value); }

        private int _cost;
        public int Cost { get => _cost; set => SetProperty(ref _cost, value); }

        // === 关键修改：在 Setter 中通知 Display 属性更新 ===

        private bool _isExtension;
        public bool IsExtension
        {
            get => _isExtension;
            set
            {
                if (SetProperty(ref _isExtension, value))
                {
                    OnPropertyChanged(nameof(ExtensionDisplay)); // 通知界面刷新显示
                }
            }
        }

        private bool _isSynthesize;
        public bool IsSynthesize
        {
            get => _isSynthesize;
            set
            {
                if (SetProperty(ref _isSynthesize, value))
                {
                    OnPropertyChanged(nameof(SynthesizeDisplay)); // 通知界面刷新显示
                }
            }
        }

        private bool _isAttack;
        public bool IsAttack
        {
            get => _isAttack;
            set
            {
                if (SetProperty(ref _isAttack, value))
                {
                    OnPropertyChanged(nameof(AttackDisplay));
                }
            }
        }

        private bool _isHeal;
        public bool IsHeal
        {
            get => _isHeal;
            set
            {
                if (SetProperty(ref _isHeal, value))
                {
                    OnPropertyChanged(nameof(HealDisplay));
                }
            }
        }

        private bool _isWeapon;
        public bool IsWeapon
        {
            get => _isWeapon;
            set
            {
                if (SetProperty(ref _isWeapon, value))
                {
                    OnPropertyChanged(nameof(WeaponDisplay));
                }
            }
        }

        private bool _isArmor;
        public bool IsArmor
        {
            get => _isArmor;
            set
            {
                if (SetProperty(ref _isArmor, value))
                {
                    OnPropertyChanged(nameof(ArmorDisplay));
                }
            }
        }

        private bool _isAccessory;
        public bool IsAccessory
        {
            get => _isAccessory;
            set
            {
                if (SetProperty(ref _isAccessory, value))
                {
                    OnPropertyChanged(nameof(AccessoryDisplay));
                }
            }
        }

        private string _description;
        public string Description { get => _description; set => SetProperty(ref _description, value); }

        private string _note;
        public string Note { get => _note; set => SetProperty(ref _note, value); }

        // === 辅助显示属性 ===
        public string ExtensionDisplay => IsExtension ? "O" : "";
        public string SynthesizeDisplay => IsSynthesize ? "O" : "";
        public string AttackDisplay => IsAttack ? "O" : "";
        public string HealDisplay => IsHeal ? "O" : "";
        public string WeaponDisplay => IsWeapon ? "O" : "";
        public string ArmorDisplay => IsArmor ? "O" : "";
        public string AccessoryDisplay => IsAccessory ? "O" : "";
    }
}
