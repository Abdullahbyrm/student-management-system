using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AspNetCoreCrudDemo.Migrations
{
    /// <inheritdoc />
    public partial class AddCoursesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    CourseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Duration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rating = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.CourseId);
                });

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "CourseId", "Category", "Description", "Duration", "ImageUrl", "Rating", "Title" },
                values: new object[,]
                {
                    { 1, "Yazılım", "Modern web uygulamaları ve güçlü API'ler geliştirmek için C# ve .NET Core ekosistemini sıfırdan zirveye öğrenin. Katmanlı mimari, Entity Framework ve Onion Architecture prensiplerine hakim olun.", "120 Saat", "/images/csharp_backend.png", 4.9000000000000004, "C# ile .NET Core (Backend)" },
                    { 2, "Web", "Günümüzün en popüler JavaScript kütüphaneleriyle uçtan uca modern ve dinamik web platformları inşa edin. MERN stack ile gerçek projeler üzerinden Full Stack yetkinliği kazanın.", "95 Saat", "/images/react_nodejs.png", 4.7999999999999998, "React & Node.js (Full Stack)" },
                    { 3, "Yapay Zeka", "Büyük veriyi analiz edin, yapay zeka modelleri kurun ve geleceğin mesleği olan veri biliminde uzmanlaşın. Pandas, NumPy ve Scikit-learn kütüphanelerini ustalıkla kullanın.", "80 Saat", "/images/python_data.png", 4.9000000000000004, "Python ile Veri Bilimi (Data Science)" },
                    { 4, "Kurumsal", "Büyük şirketlerin ve bankaların vazgeçilmez altyapısı olan Java Spring Boot ile kurumsal mimariye adım atın. Microservices ve Cloud-Native geliştirme temellerini öğrenin.", "110 Saat", "/images/java_spring.png", 4.7000000000000002, "Java Spring Boot (Kurumsal Web)" },
                    { 5, "Mobil", "Tek bir kod tabanından hem iOS hem de Android için native performansında muhteşem mobil uygulamalar geliştirin. Cross-platform dünyasında aranan bir geliştirici olun.", "85 Saat", "/images/flutter_dart.png", 4.9000000000000004, "Flutter & Dart (Mobil Uygulama)" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Courses");
        }
    }
}
