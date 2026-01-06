namespace Challenge.Queries.Common.Models;

public class OrderFieldRequest<T>
{
    public OrderFieldQueryDirection Direction { get; set; }
    public T OrderBy { get; set; }
}

public enum OrderFieldQueryDirection
{
    ASC = 0,
    DESC = 1,
}
