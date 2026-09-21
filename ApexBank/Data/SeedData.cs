using ApexBank.Data;
using ApexBank.Enums;
using ApexBank.Models;
namespace ApexBank.Data;
public static class SeedData {
 public static void Initialize(AppDbContext db){ if(db.Users.Any()) return;
  db.Users.AddRange(
   new User{Name="System Admin",Email="admin@bank.local",Phone="9000000001",DateOfBirth=new DateTime(1990,1,1),Password="1990",Role=Role.Admin,IsApproved=true},
   new User{Name="Main Teller",Email="teller@bank.local",Phone="9000000002",DateOfBirth=new DateTime(1992,2,2),Password="1992",Role=Role.Teller,IsApproved=true},
   new User{Name="Loan Officer",Email="officer@bank.local",Phone="9000000003",DateOfBirth=new DateTime(1991,3,3),Password="1991",Role=Role.LoanOfficer,IsApproved=true},
   new User{Name="Bank Auditor",Email="auditor@bank.local",Phone="9000000004",DateOfBirth=new DateTime(1989,4,4),Password="1989",Role=Role.Auditor,IsApproved=true}); db.SaveChanges();
  db.AuditLogs.Add(new AuditLog{PerformedBy="System",Role="Admin",Module="SYSTEM",ActionPerformed="INITIAL_SEED",Details="Default staff accounts created"}); db.SaveChanges();
 }
}
