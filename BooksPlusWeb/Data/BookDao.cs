using BooksPlusWeb.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BooksPlusWeb.Data;

// The data access for books and categories, with Dapper.
public class BookDao
{
    private const string BookColumns = "BookId, CategoryId, Title, PublishDate, Price, Summary";
    private readonly string connectionString;

    public BookDao(string connectionString)
    {
        this.connectionString = connectionString;
    }

    // The newest books added to the store, newest first.
    public List<Book> GetFeatured(int count)
    {
        using SqlConnection connection = new SqlConnection(connectionString);
        return connection.Query<Book>(
            $"SELECT TOP (@Count) {BookColumns} FROM Book ORDER BY BookId DESC;", new { Count = count }).ToList();
    }

    // Every book, in alphabetical order of title.
    public List<Book> GetAll()
    {
        using SqlConnection connection = new SqlConnection(connectionString);
        return connection.Query<Book>($"SELECT {BookColumns} FROM Book ORDER BY Title;").ToList();
    }

    // How many books there are.
    public int Count()
    {
        using SqlConnection connection = new SqlConnection(connectionString);
        return connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Book;");
    }

    // One page of books, in alphabetical order of title. Page 1 is the first page.
    public List<Book> GetPage(int page, int pageSize)
    {
        string sql = $"""
            SELECT {BookColumns}
            FROM Book
            ORDER BY Title
            OFFSET @Skip ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

        using SqlConnection connection = new SqlConnection(connectionString);
        return connection.Query<Book>(sql, new { Skip = (page - 1) * pageSize, PageSize = pageSize }).ToList();
    }

    // The book with this id, or null if there isn't one.
    public Book? GetById(int bookId)
    {
        using SqlConnection connection = new SqlConnection(connectionString);
        return connection.QuerySingleOrDefault<Book>(
            $"SELECT {BookColumns} FROM Book WHERE BookId = @BookId;", new { BookId = bookId });
    }

    // The books with these ids, in any order. Ids with no book are skipped.
    public List<Book> GetByIds(List<int> bookIds)
    {
        using SqlConnection connection = new SqlConnection(connectionString);
        return connection.Query<Book>(
            $"SELECT {BookColumns} FROM Book WHERE BookId IN @BookIds;", new { BookIds = bookIds }).ToList();
    }

    // Every book whose title contains the text, in alphabetical order of title.
    public List<Book> SearchByTitle(string text)
    {
        using SqlConnection connection = new SqlConnection(connectionString);
        return connection.Query<Book>(
            $"SELECT {BookColumns} FROM Book WHERE Title LIKE '%' + @Text + '%' ORDER BY Title;", new { Text = text }).ToList();
    }

    // Whether a book with exactly this title exists.
    public bool TitleExists(string title)
    {
        using SqlConnection connection = new SqlConnection(connectionString);
        return connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Book WHERE Title = @Title;", new { Title = title }) > 0;
    }

    // Adds a book and returns its new BookId.
    public int Add(Book book)
    {
        string sql = """
            INSERT INTO Book (CategoryId, Title, PublishDate, Price, Summary)
            VALUES (@CategoryId, @Title, @PublishDate, @Price, @Summary);
            SELECT CAST(SCOPE_IDENTITY() AS int);
            """;

        using SqlConnection connection = new SqlConnection(connectionString);
        return connection.QuerySingle<int>(sql, book);
    }

    // Every category, in alphabetical order of name.
    public List<Category> GetCategories()
    {
        using SqlConnection connection = new SqlConnection(connectionString);
        return connection.Query<Category>("SELECT CategoryId, CategoryName FROM Category ORDER BY CategoryName;").ToList();
    }
}
