namespace TodoList.Exceptions;

public class NotFoundException(string message) : Exception(message) { }
