using Backend.Models;

namespace Backend;

public interface IPrinter
{
    public void Print(int transactionId);
}