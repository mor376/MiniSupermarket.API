using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniSupermarket.WinForms
{
    public static class RolePermissionCatalog
    {
        private static readonly Dictionary<string, HashSet<string>> _rolePermissions = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Admin"] = new(StringComparer.OrdinalIgnoreCase)
            {
                "POS", "CATEGORY", "PRODUCT", "CUSTOMER", "REPORTS", "USER_MANAGE"
            },
            ["Cashier"] = new(StringComparer.OrdinalIgnoreCase)
            {
                "POS", "CUSTOMER"
            },
            ["Warehouse"] = new(StringComparer.OrdinalIgnoreCase)
            {
                "CATEGORY", "PRODUCT"
            }
        };

        private static readonly List<DemoAccount> _accounts = new()
        {
            new() { UserId = 1, Username = "admin01", Password = "123456", FullName = "Nguyễn Quản Trị", Role = "Admin", IsActive = true },
            new() { UserId = 2, Username = "admin02", Password = "123456", FullName = "Trần Giám Đốc", Role = "Admin", IsActive = true },
            new() { UserId = 3, Username = "cashier01", Password = "123456", FullName = "Lê Thu Ngân", Role = "Cashier", IsActive = true },
            new() { UserId = 4, Username = "cashier02", Password = "123456", FullName = "Phạm Bán Hàng", Role = "Cashier", IsActive = true },
            new() { UserId = 5, Username = "cashier03", Password = "123456", FullName = "Hoàng Thu Ngân", Role = "Cashier", IsActive = true },
            new() { UserId = 6, Username = "cashier04", Password = "123456", FullName = "Vũ Thị Quầy", Role = "Cashier", IsActive = true },
            new() { UserId = 7, Username = "cashier05", Password = "123456", FullName = "Đỗ Bán Lẻ", Role = "Cashier", IsActive = true },
            new() { UserId = 8, Username = "ware01", Password = "123456", FullName = "Ngô Quản Kho", Role = "Warehouse", IsActive = true },
            new() { UserId = 9, Username = "ware02", Password = "123456", FullName = "Bùi Kiểm Kê", Role = "Warehouse", IsActive = true },
            new() { UserId = 10, Username = "ware03", Password = "123456", FullName = "Dương Thủ Kho", Role = "Warehouse", IsActive = true },
            new() { UserId = 11, Username = "ware04", Password = "123456", FullName = "Lý Nhập Hàng", Role = "Warehouse", IsActive = true },
            new() { UserId = 12, Username = "admin_backup", Password = "123456", FullName = "Đặng Hỗ Trợ", Role = "Admin", IsActive = true },
            new() { UserId = 13, Username = "cashier06", Password = "123456", FullName = "Hồ Ca Chiều", Role = "Cashier", IsActive = true },
            new() { UserId = 14, Username = "ware05", Password = "123456", FullName = "Trương Vận Chuyển", Role = "Warehouse", IsActive = true },
            new() { UserId = 15, Username = "supervisor", Password = "123456", FullName = "Mai Giám Sát", Role = "Admin", IsActive = true }
        };

        public sealed class DemoAccount
        {
            public int UserId { get; set; }
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string FullName { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;
            public bool IsActive { get; set; } = true;
        }

        public static IReadOnlyList<DemoAccount> Accounts => _accounts;

        public static DemoAccount? FindDemoAccount(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            return _accounts.FirstOrDefault(account =>
                account.IsActive &&
                string.Equals(account.Username, username.Trim(), StringComparison.OrdinalIgnoreCase) &&
                string.Equals(account.Password, password.Trim(), StringComparison.Ordinal));
        }

        public static string NormalizeRole(string? role)
        {
            return (role ?? string.Empty).Trim();
        }

        public static IReadOnlyList<string> GetAllRoles()
        {
            return _rolePermissions.Keys.OrderBy(x => x).ToList();
        }

        public static IReadOnlyCollection<string> GetPermissionsForRole(string? role)
        {
            var normalized = NormalizeRole(role);
            if (string.IsNullOrEmpty(normalized))
            {
                return Array.Empty<string>();
            }

            if (_rolePermissions.TryGetValue(normalized, out var permissions))
            {
                return permissions.OrderBy(x => x).ToList();
            }

            return GetFallbackPermissions(normalized);
        }

        public static IReadOnlyCollection<string> GetEffectivePermissions(string? role)
        {
            var normalized = NormalizeRole(role);
            if (string.IsNullOrEmpty(normalized))
            {
                return Array.Empty<string>();
            }

            if (_rolePermissions.TryGetValue(normalized, out var permissions))
            {
                return new HashSet<string>(permissions, StringComparer.OrdinalIgnoreCase);
            }

            return GetFallbackPermissions(normalized);
        }

        private static IReadOnlyCollection<string> GetFallbackPermissions(string role)
        {
            if (role.Equals("ADMIN", StringComparison.OrdinalIgnoreCase)
                || role.Equals("ADMINISTRATOR", StringComparison.OrdinalIgnoreCase)
                || role.Equals("SUPERVISOR", StringComparison.OrdinalIgnoreCase))
            {
                return new[] { "POS", "CATEGORY", "PRODUCT", "CUSTOMER", "REPORTS", "USER_MANAGE" };
            }

            if (role.Equals("CASHIER", StringComparison.OrdinalIgnoreCase))
            {
                return new[] { "POS", "CUSTOMER" };
            }

            if (role.Equals("WAREHOUSE", StringComparison.OrdinalIgnoreCase)
                || role.Equals("WAREHOUSEMAN", StringComparison.OrdinalIgnoreCase)
                || role.Equals("STOCK", StringComparison.OrdinalIgnoreCase))
            {
                return new[] { "CATEGORY", "PRODUCT" };
            }

            return new[] { "POS", "CATEGORY", "PRODUCT", "CUSTOMER", "REPORTS", "USER_MANAGE" };
        }

        public static bool HasPermission(string? role, string permission)
        {
            return GetEffectivePermissions(role)
                .Any(p => p.Equals(permission, StringComparison.OrdinalIgnoreCase));
        }

        public static void AddRole(string roleName)
        {
            var value = NormalizeRole(roleName);
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Tên vai trò không được để trống.");
            }

            if (!_rolePermissions.ContainsKey(value))
            {
                _rolePermissions[value] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }
        }

        public static bool RenameRole(string oldRoleName, string newRoleName)
        {
            var currentRole = NormalizeRole(oldRoleName);
            var updatedRole = NormalizeRole(newRoleName);

            if (string.IsNullOrWhiteSpace(currentRole) || string.IsNullOrWhiteSpace(updatedRole))
            {
                return false;
            }

            if (string.Equals(currentRole, updatedRole, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (!_rolePermissions.ContainsKey(currentRole))
            {
                return false;
            }

            var permissions = new HashSet<string>(_rolePermissions[currentRole], StringComparer.OrdinalIgnoreCase);
            _rolePermissions.Remove(currentRole);
            _rolePermissions[updatedRole] = permissions;

            foreach (var account in _accounts.Where(a => a.Role.Equals(currentRole, StringComparison.OrdinalIgnoreCase)))
            {
                account.Role = updatedRole;
            }

            return true;
        }

        public static bool DeleteRole(string roleName)
        {
            var cleanRole = NormalizeRole(roleName);
            if (string.IsNullOrWhiteSpace(cleanRole) || !_rolePermissions.ContainsKey(cleanRole))
            {
                return false;
            }

            _rolePermissions.Remove(cleanRole);
            foreach (var account in _accounts.Where(a => a.Role.Equals(cleanRole, StringComparison.OrdinalIgnoreCase)))
            {
                account.Role = "Admin";
            }

            return true;
        }

        public static bool AddAccount(string username, string fullName, string role, string password)
        {
            var cleanUsername = (username ?? string.Empty).Trim();
            var cleanFullName = (fullName ?? string.Empty).Trim();
            var cleanRole = NormalizeRole(role);
            var cleanPassword = (password ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(cleanUsername) || string.IsNullOrWhiteSpace(cleanFullName) || string.IsNullOrWhiteSpace(cleanRole) || string.IsNullOrWhiteSpace(cleanPassword))
            {
                return false;
            }

            if (_accounts.Any(a => string.Equals(a.Username, cleanUsername, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            _accounts.Add(new DemoAccount
             {
                 UserId = _accounts.Count == 0 ? 1 : _accounts.Max(a => a.UserId) + 1,
                 Username = cleanUsername,
                 FullName = cleanFullName,
                 Role = cleanRole,
                 Password = cleanPassword,
                 IsActive = true
             });

            return true;
        }

        public static bool ToggleLockAccount(string username)
        {
            var account = _accounts.FirstOrDefault(a => string.Equals(a.Username, (username ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase));
            if (account == null)
            {
                return false;
            }

            account.IsActive = !account.IsActive;
            return true;
        }

        public static bool DeleteAccount(string username)
        {
            var account = _accounts.FirstOrDefault(a => string.Equals(a.Username, (username ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase));
            if (account == null)
            {
                return false;
            }

            _accounts.Remove(account);
            return true;
        }

        public static void UpdatePermissionsForRole(string roleName, IEnumerable<string> permissions)
        {
            var cleanRole = NormalizeRole(roleName);
            if (string.IsNullOrEmpty(cleanRole))
            {
                return;
            }

            if (!_rolePermissions.ContainsKey(cleanRole))
            {
                _rolePermissions[cleanRole] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }

            _rolePermissions[cleanRole].Clear();
            foreach (var permission in permissions.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrWhiteSpace(permission))
                {
                    _rolePermissions[cleanRole].Add(permission.Trim());
                }
            }
        }

        public static void ResetRole(string roleName)
        {
            var cleanRole = NormalizeRole(roleName);
            if (string.IsNullOrEmpty(cleanRole))
            {
                return;
            }

            switch (cleanRole)
            {
                case "Admin":
                    UpdatePermissionsForRole(cleanRole, new[] { "POS", "CATEGORY", "PRODUCT", "CUSTOMER", "REPORTS", "USER_MANAGE" });
                    break;
                case "Cashier":
                    UpdatePermissionsForRole(cleanRole, new[] { "POS", "CUSTOMER" });
                    break;
                case "Warehouse":
                    UpdatePermissionsForRole(cleanRole, new[] { "CATEGORY", "PRODUCT" });
                    break;
                default:
                    UpdatePermissionsForRole(cleanRole, Array.Empty<string>());
                    break;
            }
        }
    }
}
