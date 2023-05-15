using NLog;
using System.Linq;
using EJS_DOTNET_M12_D1.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

// See https://aka.ms/new-console-template for more information
string path = Directory.GetCurrentDirectory() + "\\nlog.config";

// create instance of Logger
var logger = LogManager.LoadConfiguration(path).GetCurrentClassLogger();
logger.Info("Program started");

try
{
    var db = new NWConsoleContext();
    string choice;
    do
    {
        centeredText("Welcome to the Northwinds Traders database");
        Console.WriteLine("\n\t[1] Display Categories \n\t[2] Modify Categorys table \n\t[3] Display all Products and Categorys\n\t[4] Display a Product \n\t[5] Modify Products \n\t[6] Save changes to the Database");
        centeredText("[0] to quit");
        choice = Console.ReadLine();
        Console.Clear();
        logger.Info($"Option {choice} selected");

        try
        {
            Console.Clear();
            switch (Int32.Parse(choice))
            {

                case 1: // Display categories

                    centeredText("What would you like to Display");
                    Console.WriteLine("\n\t [1] Display Categories \n\t [2] Display Category and related products \n");

                    string R1 = Console.ReadLine();
                    logger.Info($"Choice {R1} selected for Display categories");
                    switch (R1)
                    {
                        case "1":

                            Console.Clear();
                            var Q1A = db.Categories.OrderBy(p => p.CategoryName);


                            centeredText($"{Q1A.Count()} records returned");

                            foreach (var item in Q1A)
                            {
                                Console.WriteLine($"{item.CategoryName} - {item.Description}");
                            }


                            keyToContinue();
                            break;


                        case "2":
                            Console.Clear();


                            var Q1B = db.Categories.OrderBy(p => p.CategoryId);

                            Console.WriteLine("Select the category whose products you want to display:");
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            foreach (var item in Q1B)
                            {
                                Console.WriteLine($"\t[{item.CategoryId}] {item.CategoryName}");
                            }
                            Console.ForegroundColor = ConsoleColor.White;
                            int id = int.Parse(Console.ReadLine());
                            Console.Clear();
                            logger.Info($"CategoryId {id} selected");
                            Category category = db.Categories.Include("Products").FirstOrDefault(c => c.CategoryId == id);
                            Console.WriteLine($"{category.CategoryName} - {category.Description}");
                            foreach (Product p in category.Products)
                            {
                                Console.WriteLine($"\t{p.ProductName}");
                            }

                            keyToContinue();

                            break;

                        default:
                            logger.Error($"Invalid input");
                            keyToContinue();
                            break;
                    }

                    break;

                case 2: // add, delete or edit part of the categorys table
                    Console.Clear();
                    centeredText("How would you like to modify Categories");
                    Console.WriteLine("\n\t [1] Add Categories \n\t [2] Delete Category and related products \n\t [3] Edit Categories");
                    switch (Console.ReadLine())
                    {
                        case "1":
                            Category newCategory = addCategoryValidation(db, logger);
                            if (newCategory != null)
                            {
                                db.AddCategory(newCategory);
                                logger.Info("Category added - {name}", newCategory.CategoryName);
                            }


                            break;

                        case "2":
                            Console.Clear();
                            centeredText("Choose a category to delete");
                            var category = getCategory(db, logger);
                            if (category != null)
                            {

                                db.DeleteCatagory(category);
                                logger.Info($"Catagory ID: {category.CategoryId}  Name: {category.CategoryName} along with all related products has been deleted");


                            }

                            break;

                        case "3":

                            Console.WriteLine("Choose a Category to edit");
                            var EditCategroy = getCategory(db, logger);
                            if (EditCategroy != null)
                            {
                                Category updatedCategory = addCategoryValidation(db, logger);
                                if (updatedCategory != null)
                                {
                                    updatedCategory.CategoryId = EditCategroy.CategoryId;
                                    db.EditCategroy(updatedCategory);
                                    logger.Info($"Category ID: {EditCategroy.CategoryId} has been updated");
                                }
                            }

                            break;


                    }








                    break;


                case 3:  // just a display all 

                    Console.Clear();
                    Console.WriteLine("Would you like to see: \n\t[1] all products \n\t[2] Active products \n\t[3] Inactive products");
                    var Q3A = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
                    switch (Console.ReadLine())
                    {
                        case "1":

                            bufferArea(200);
                            foreach (var item in Q3A)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                centeredText($"{item.CategoryName}");
                                centeredText($"-----------------------------------------------------------------------------------------------------------");
                                Console.ForegroundColor = ConsoleColor.White;
                                foreach (Product p in item.Products)
                                {
                                    if (p.Discontinued == true)
                                    {
                                        Console.WriteLine($"\t{p.ProductName} - Discontinued");
                                    }
                                    else { Console.WriteLine($"\t{p.ProductName}"); }

                                }
                                Console.WriteLine();

                            }


                            break;


                        case "2":


                            bufferArea(200);
                            foreach (var item in Q3A)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                centeredText($"{item.CategoryName}");
                                centeredText($"-----------------------------------------------------------------------------------------------------------");
                                Console.ForegroundColor = ConsoleColor.White;
                                foreach (Product p in item.Products.Where(p => p.Discontinued == false))
                                {
                                    Console.WriteLine($"\t{p.ProductName}");
                                }
                                Console.WriteLine();

                            }


                            break;

                        case "3":

                            bufferArea(200);
                            foreach (var item in Q3A)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                centeredText($"{item.CategoryName}");
                                centeredText($"-----------------------------------------------------------------------------------------------------------");
                                Console.ForegroundColor = ConsoleColor.White;
                                foreach (Product p in item.Products.Where(p => p.Discontinued == true))
                                {
                                    Console.WriteLine($"\t{p.ProductName} \tDiscontinued {p.Discontinued}");
                                }
                                Console.WriteLine();

                            }


                            break;


                        default:

                            logger.Error("Invalid input please select 1 2 or 3");

                            break;

                    }


                    keyToContinue();


                    break;

                case 4: // Displaying products


                    Console.Clear();

                    var Q4B1 = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
                    bufferArea(500);
                    foreach (var item in Q4B1)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        centeredText($"{item.CategoryName}");
                        centeredText($"-----------------------------------------------------------------------------------------------------------");
                        Console.ForegroundColor = ConsoleColor.White;
                        foreach (Product p in item.Products)
                        {

                            Console.WriteLine($"[{p.ProductId}] {p.ProductName}");
                        }
                        Console.WriteLine();

                    }
                    centeredText("Select which product you would like to display {Select via the [#] number}");


                    try
                    {
                        int R4B1 = Int32.Parse(Console.ReadLine());
                        logger.Info($"product ID: {R4B1} chosen");
                        //db.Categories.Include("Products").FirstOrDefault(c => c.CategoryId == id);
                        Product Q4B2 = db.Products.FirstOrDefault(c => c.ProductId == R4B1);
                        Console.Clear();
                        colourRed();
                        centeredText(Q4B2.ProductName);
                        colourWhite();
                        Console.WriteLine($"\n\x1b[1mID:\x1b[0m {Q4B2.ProductId} \n\x1b[1mSupplier ID:\x1b[0m {Q4B2.SupplierId} \n\x1b[1mCategory ID:\x1b[0m {Q4B2.CategoryId} \n\x1b[1mBatch size:\x1b[0m {Q4B2.QuantityPerUnit} \n\x1b[1mPrice per Unit:\x1b[0m {Q4B2.UnitPrice:C2} \n\x1b[1mUnits in Stock:\x1b[0m {Q4B2.UnitsInStock} \n\x1b[1mUnits on Order:\x1b[0m {Q4B2.UnitsOnOrder} \n\x1b[1mReorder Level:\x1b[0m {Q4B2.ReorderLevel} \n\x1b[1mDiscontinued:\x1b[0m {Q4B2.Discontinued}");
                        keyToContinue();
                    }
                    catch
                    {

                    }





                    break;

                case 5: // add, delete or edit part of the products table
                    Console.Clear();
                    centeredText("How would you like to modify Categories");
                    Console.WriteLine("\n\t [1] Add product \n\t [2] Delete product and related orders \n\t [3] Edit products");
                    switch (Console.ReadLine())
                    {
                        case "1":
                            Console.Clear();
                            Product newProduct = validateInputProduct(db, logger);
                            if (newProduct != null)
                            {
                                db.AddProduct(newProduct);
                                logger.Info($"New product added ID: {newProduct.ProductId} Name: {newProduct.ProductName}");
                            }

                            break;


                        case "2":
                            Console.Clear();
                            centeredText("Which product would you like to delete");
                            Product deleteProduct = getProduct(db, logger);
                            if (deleteProduct != null)
                            {
                                logger.Info($"product ID: {deleteProduct.ProductId} Name: {deleteProduct.ProductName} has been deleted");
                                db.DeleteProduct(deleteProduct);
                            }


                            break;


                        case "3":

                            Console.WriteLine("Choose a product to edit");
                            var EditProduct = getProduct(db, logger);
                            if (EditProduct != null)
                            {
                                Product updatedProduct = validateInputProduct(db, logger);
                                if (updatedProduct != null)
                                {
                                    updatedProduct.ProductId = EditProduct.ProductId;
                                    db.EditProduct(updatedProduct);
                                    logger.Info($"Category ID: {EditProduct.ProductId} has been updated");
                                }
                            }



                            break;


                        default:


                            break;
                    }



                    break;


                case 6:

                    db.SaveDatabase();

                    logger.Info("Database saved");

                    break;

            }
        }
        catch
        {
            logger.Error($"Invalid input of {choice}");
        }




    } while (choice.ToLower() != "0");
}
catch (Exception ex)
{
    logger.Error(ex.Message);
}

logger.Info("Program ended");


// Methods for interface stuff

void centeredText(string text)
{
    Console.WriteLine(String.Format("{0," + ((Console.WindowWidth / 2) + (text.Length / 2)) + "}", text) + "\n");
}


void keyToContinue()
{
    Console.ForegroundColor = ConsoleColor.Red;
    centeredText("Press any key to continue");
    Console.ReadKey();
    Console.ForegroundColor = ConsoleColor.White;
    Console.Clear();
}

void bufferArea(int bufferAmount)
{

    for (int i = 0; i > bufferAmount; i++)
    {
        Console.WriteLine("\n");
    }

}

void colourRed()
{
    Console.ForegroundColor = ConsoleColor.Red;
}
void colourWhite()
{
    Console.ForegroundColor = ConsoleColor.White;
}


// Methods for function 
static Category addCategoryValidation(NWConsoleContext db, Logger logger)
{

    Category category = new Category();
    Console.WriteLine("Enter the name of the caregory");
    category.CategoryName = Console.ReadLine();
    Console.WriteLine("Enter the description of the caregory");
    category.Description = Console.ReadLine();
    ValidationContext context = new ValidationContext(category, null, null);
    List<ValidationResult> results = new List<ValidationResult>();
    var isValid = Validator.TryValidateObject(category, context, results, true);
    if (isValid)

        // prevent duplicates

        if (db.Categories.Any(c => c.CategoryName == category.CategoryName))
        {

            results.Add(new ValidationResult("Category name exists", new string[] { "Name" }));
        }
        else
        {
            return category;
        }

    foreach (var result in results)
    {
        logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
    }

    return null;

}

static Category getCategory(NWConsoleContext db, Logger logger)
{

    var categories = db.Categories.OrderBy(c => c.CategoryId);
    foreach (Category c in categories)
    {
        Console.WriteLine($"[{c.CategoryId}] {c.CategoryName}");

    }
    if (int.TryParse(Console.ReadLine(), out int CategoryId))
    {
        Category category = db.Categories.FirstOrDefault(c => c.CategoryId == CategoryId);
        if (category != null)
        {
            return category;
        }
    }
    logger.Error($"{CategoryId} is an invalid category ID");
    return null;
}

static Product getProduct(NWConsoleContext db, Logger logger)
{

    var cat = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
    foreach (var item in cat)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"{item.CategoryName}");
        Console.WriteLine($"-----------------------------------------------------------------------------------------------------------");
        Console.ForegroundColor = ConsoleColor.White;
        foreach (Product p in item.Products)
        {

            Console.WriteLine($"[{p.ProductId}] {p.ProductName}");
        }
        Console.WriteLine();

    }
    Console.WriteLine("Select which product you would like {Select via the [#] number}");
    if (int.TryParse(Console.ReadLine(), out int ProductID))
    {
        Product product = db.Products.FirstOrDefault(p => p.ProductId == ProductID);
        if (product != null)
        {
            logger.Info($"Product Succsessfully returned (product ID: {product.ProductId} Product Name: {product.ProductName})");
            return product;

        }
        else
        {
            logger.Error($"{ProductID} product does not exist");
        }
    }

    logger.Error($"{ProductID} is an invalid product ID");
    return null;

}

static Product validateInputProduct(NWConsoleContext db, Logger logger)
{

    Product product = new Product();
    Console.Clear();
    Console.WriteLine("\n What is the name of the product \n ->");
    product.ProductName = Console.ReadLine();
    var MQP1 = db.Categories.OrderBy(p => p.CategoryId);
    logger.Info($"{MQP1.Count()} records returned");
    foreach (var item in MQP1)
    {
        Console.WriteLine($"[{item.CategoryId}] {item.CategoryName}");
    }
    Console.WriteLine($"Select which category it {product.ProductName} listed under (Select via the [#] number)");
    try
    {
        product.CategoryId = Int32.Parse(Console.ReadLine());

        Console.WriteLine($"Who is the supplier of the {product.ProductName} (Select via the [#] number)");
        var MQP2 = db.Suppliers.OrderBy(s => s.SupplierId);
        logger.Info($"{MQP2.Count()} records returned");
        foreach (var item in MQP2)
        {
            Console.WriteLine($"[{item.SupplierId}] {item.CompanyName}");
        }
        try
        {
            product.SupplierId = Int32.Parse(Console.ReadLine());
            product.Discontinued = false;
            product.UnitsOnOrder = 0;
            product.UnitsInStock = 0;
            Console.WriteLine($"How much does {product.ProductName} cost per unit (Do not use $ or any other currency symbol)");
            try
            {
                product.UnitPrice = Decimal.Parse(Console.ReadLine());
                Console.WriteLine($"How much of {product.ProductName} is in one unit");
                product.QuantityPerUnit = Console.ReadLine();

                ValidationContext context = new ValidationContext(product, null, null);
                List<ValidationResult> results = new List<ValidationResult>();

                var isValid = Validator.TryValidateObject(product, context, results, true);

                if (isValid)
                {

                    return product;

                }
                foreach (var result in results)
                {
                    logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                }


            }
            catch
            {

                logger.Error("Invalid product cost");

            }


        }
        catch
        {

            logger.Error("Invalid product supplier ID");


        }



    }
    catch
    {

        logger.Error("Invalid product Category ID");

    }

    return null;


}