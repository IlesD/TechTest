# TechTest

## Back End Tech Assessment - C# / .Net

Create a solution to contain an HTTP endpoint that:
1. Accepts a GET request with three optional query parameters to filter products or
highlight some words (separated by spaces in the query parameter) in their description:
Example: /filter?maxprice=20&size=medium&highlight=green,blue
2. Reads the list of all products from the URL (think of this as the database): {URL removed}
3. Design the endpoint response so that it contains (in JSON format):
   - All products if the request has no parameters
   - A filtered subset of products if the request has filter parameters
   - A filter object to contain:
     - The minimum and the maximum price of all products in the source URL
     - An array of strings to contain all sizes of all products in the source URL
     - An string array of size ten to contain most common words in the product descriptions, excluding the most common five
   - Add HTML tags to returned product descriptions in order to highlight the words provided in the highlight parameter. Example: “These trousers make a perfect pair with <em>green</em> or <em>blue</em> shirts.”
