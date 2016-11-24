# RestLibrary

More often than not, when we need to talk to some kind of REST Endpoints, we end up doing the same boilerplate activities: perform OAuth login, set authorization header, make GET, POST, PUT, DELETE HTTP calls with objects that need to be serialized/deserialized in JSON format.

**RestLibrary** aims to greatly simplify all these tasks: it is a lightweight library to perform common REST API calls. It is NOT intended to be a universal REST client that fits every needs. It has been developed to address requests that tipically we have to manage, in particular when we're using ASP .NET Web API as backend.

**Installation**

The library is available on [NuGet](https://www.nuget.org/packages/RestLibrary/). Just search *RestLibrary* in the **Package Manager GUI** or run the following command in the **Package Manager Console**:

    Install-Package RestLibrary
    
**Usage**

    var client = new RestClient("http://localhost:27243/");
    var isLoggedIn = await client.OAuthLoginAsync("demo@demo.com", "password");
            
    var productListResponse = await client.GetAsync<IEnumerable<Product>>("api/products");
    if (productListResponse.IsSuccessful)
    {   
        foreach (var product in productListResponse.Content) 
        {      
            PrintProductInformation(product);  
        }
    }
    
    var productResponse = await client.GetAsync<Product>("api/products/96");
    if (productResponse.IsSuccessful)
    {    
        PrintProductInformation(productResponse.Content);
    }
    else if (productResponse.StatusCode == HttpStatusCode.NotFound)
    {
        Debug.WriteLine("Product not found.");
    }
    
    var newProduct = new Product { Name = "Mouse Bluetooth", Price = 25 };
    var newProductResponse = await client.PostWithResultAsync<Product, Product>("api/products", newProduct);
    Debug.WriteLine($"The ID of the new product is {newProductResponse.Content.Id}");

Other samples are available in the [Samples](https://github.com/marcominerva/RestLibrary/tree/master/Samples) folder.

**Contribute**

The project is continually evolving. We welcome contributions. Feel free to file issues and pull requests on the repo and we'll address them as we can.
