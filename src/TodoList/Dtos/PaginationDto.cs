namespace TodoList.Dtos;

public record PaginationDto<T>(int PageNumber, int PageSize, int TotalItems, IEnumerable<T> Items);
