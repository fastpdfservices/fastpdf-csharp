# FastPDF C# Package (.NET 6.0)

Welcome to **fastpdf** C# Package, the versatile and quick solution for your PDF rendering needs. This package is a SDK for fastpdfservice.com REST API. Requires .NET 6.0 or newer.

## Overview

`fastpdf` is a C# package designed to simplify the PDF generation process. It streamlines the procedure of rendering HTML/CSS templates into a PDF document, offering granular control over the rendering options, and provides the ability to add images and barcodes into your PDFs.

`fastpdf` leverages the PDF manipulation API available at [fastpdfservice.com](https://fastpdfservice.com), ensuring high performance, scalability, and reliability. By using fastpdf, you are able to generate complex, high-quality PDFs efficiently. 

## Key Features

1. **HTML/CSS to PDF**: Render your HTML/CSS templates into a PDF with ease and precision.
2. **Header/Footer Management**: Easily manage the header and footer of your documents with flexible rendering options.
3. **Barcode & Image Integration**: Seamlessly integrate images and barcodes into your PDFs.
4. **Render Options**: Advanced render options allow granular control over your output. Control aspects like margins, orientation, scales, etc.
5. **Image Mode Selection**: fastpdf supports various image modes to tailor the image rendering to your needs.

## Getting Started

Before you start with the fastpdf C# Package, make sure you have .NET 6.0 installed on your machine.

To use `fastpdf`, you'll first need to register an account to [fastpdfservice.com](https://fastpdfservice.com), and get your API key. 

Install fastpdf from NuGet:
```xml {{ title: 'C#' }}
// Add this package reference in your .csproj file
<PackageReference Include="FastPDFService" Version="1.0.0" />
```

Import the package in your C# file:

```C#
using FastPDFService;
```

To start with, make sure you have your FastPDF service API token handy. 
This is required for all API calls to the FastPDF service. You can find your token under API settings in 
your [FastPDF profile](https://fastpdfservice.com/profile/api).

<CodeGroup title="Register your token">

```C# {{ title: 'C#' }}
PDFClient client = new PDFClient("API_KEY");
```

</CodeGroup>


Please refer to the [documentation](https://docs.fastpdfservice.com) for more details on the available features.


## Creating a Basic Template

Creating a basic template is simple. Let's start by creating an HTML document with a placeholder for a `name` variable.
Then, you'll need to create a [Template](https://docs.fastpdfservice.com/templates#template) object and set `name` and `format`.


<CodeGroup title="Creating a basic template">

```C# {{ title: 'C#' }}
using FastPDFService.Models;

string templateStr = "<html><body>Hello world, my name is {{name}}!</body></html>";
Template templateData = new Template { Name = "basic-document", Format = "html" };
Template template = await client.AddTemplateAsync(templateStr, templateData);
```

</CodeGroup>

<Note>
  *Make sure you do not forget to call `getBytes()` on your String. Otherwise, the string will be interpreted as a filename.*
</Note>

In this template, `{{name}}` is a placeholder that will be replaced by a real value when generating a PDF. 

## Your First Rendering

Now, let's bring our document to life. We've got our template id handy, and our data ready to be cast into the chosen template. 
But don't worry if you've misplaced the template id - a quick detour to your [template dashboard](https://fastpdfservice.com/services/templates) will point you right to it.
In our data object, we set a value for the `name` variable that we defined in our template.

<CodeGroup title="Rendering our basic template">

```C# {{ title: 'C#' }}
var documentData = new Dictionary<string, object>
{
    {"name", "John"}
};

string templateId = template.Id;
byte[] document = await client.RenderTemplateAsync(templateId, documentData);
```

</CodeGroup>

Finally, we save it to the disk using the `save()` convenience function.

<CodeGroup title="Saving our document to the disk">

```C# {{ title: 'C#' }}
await client.SaveAsync(document, "path/to/basic-document.pdf");
```

</CodeGroup>

And that's it! You've just brought your first PDF to life.


## For Loops

FastPDF allows for more complex templates. For instance, you can use control structures like for loops and if statements in your templates.
Here is how to iterate over a list of items, using a for loop.

<CodeGroup title="For Loop">

```C# {{ title: 'C#' }}
string templateContent = @"
<html>
<body>
    <ul>
    {% for item in items %}
        <li>{{ item }}</li>
    {% endfor %}
    </ul>
</body>
</html>
";
Template templateData = new Template { Name = "for-loop-document", Format = "html" };
Template template = await client.AddTemplateAsync(templateContent, templateData);
```

</CodeGroup>

In this template, `{% for item in items %}` starts the loop, and `{% endfor %}` ends it.

Next, we define our renderingData by associating a list of item value to the `items` key, and render our document.

<CodeGroup title="Rendering our For Loop template">

```C# {{ title: 'C#' }}
var renderingData = new Dictionary<string, object>
{
    {"items", new List<string> {"First item", "Second item", "Last item"}}
};

string templateId = template.Id;
byte[] document = await client.RenderTemplateAsync(templateId, renderingData);
await client.SaveAsync(document, "path/to/loop-document.pdf");
```

</CodeGroup>


## If Statements

If statements can be used to conditionally include parts of the template, based on the rendering data. Here's an example:

<CodeGroup title="If statement">

```C# {{ title: 'C#' }}
string templateContent = @"
<html>
<body>
    {% if user_premium %}
        <p>Welcome, premium user!</p>
    {% else %}
        <p>Welcome, regular user!</p>
    {% endif %}
</body>
</html>
";
Template templateData = new Template { Name = "if-document", Format = "html" };
Template template = await client.AddTemplateAsync(templateContent, templateData);
```

</CodeGroup>

In this example, `{% if user_premium %}` starts the if statement, and `{% endif %}` ends it. 
If `user_premium` is true, the "Welcome, premium user!" message will be included in the generated PDF. 
Otherwise, the "Welcome, regular user!" message will be included.

## Stylesheets

<Note>
    *Please ensure that your stylesheets are written purely in CSS. As of now, FastPDF does not support precompiled CSS languages 
    like SCSS, or CSS frameworks like Tailwind. 
    Stick to standard CSS to ensure compatibility and successful rendering.*
</Note>

You can do this in one of two ways: by using a separate stylesheet, or by using inline CSS. 

The latter works as usual, by either
setting the `style` property html tags, or by writing your css classes in the header of the template:

<CodeGroup title="A template with style">

```html {{ title: 'HTML' }}
<html>
<head>
    my-class {
        color: #f43f54;
        font-size: 12px;
        font-weight: bold;
    }
</head>
<body>
    <p class='my-class'>Welcome, {{name}}!</p>
    <p style='color: #ccc'> Goodbye ! </p>
</body>
</html>
```

</CodeGroup>

Alternatively, you can use the `AddStylesheetAsync()` method to add a stylesheet to your template:

<CodeGroup title="A template with style">

```C# {{ title: 'C#' }}
string stylesheetPath = "styles.css";
StyleFile stylesheetData = new StyleFile { Format = "css" };

string templateId = template.Id;
await client.AddStylesheetAsync(templateId, stylesheetPath, stylesheetData);
```

</CodeGroup>

Your template will now be rendered with style !

## Adding an image

FastPDF allows you to add images to your templates, which can then be rendered into your final PDF documents. Here's how you can accomplish this.

To begin, we create our template with an `<img>` tag, using our image uri in the `src` property as followed:

<CodeGroup title="Creating a template with an image">

```C# {{ title: 'C#' }}
String template_content = """
<html>
<body>
    <img src="{{my_favourite_logo}}">
    <p>Welcome, {{name}}!</p>
</body>
</html>
""";
Template templateData = new Template { Name = "image-document", Format = "html" };
Template template = await client.AddTemplateAsync(templateContent, templateData);
```

</CodeGroup>

Then, we add an image to our template, and set the same image uri. 
This step can be done from your [template dashboard](https://fastpdfservice.com/services/templates).

<CodeGroup title="Adding an image">

```C# {{ title: 'C#' }}
string imagePath = "my-logo.png";
ImageFile imageData = new ImageFile { Format = "png", Uri = "my_favourite_logo" };

string templateId = template.Id;
await client.AddImageAsync(templateId, imagePath, imageData);
```

</CodeGroup>

Finally, we render our document. As usual, we define our rendering data object with our variable. 

<CodeGroup title="Rendering our image template">

```C# {{ title: 'C#' }}
var documentData = new Dictionary<string, object>
{
    {"name", "Jane"}
};

string templateId = template.Id;
byte[] document = await client.RenderTemplateAsync(templateId, documentData);
// Save to disk, if needed
await client.SaveAsync(document, "path/to/image-document.pdf");
```

</CodeGroup>

## Custom Header and Footer

By default, FastPDF will include a header, composed of the document title and the date, and a footer with page numbers. 
You can turn them on and off separately when adding your [Template](https://docs.fastpdfservice.com/templates#template) or during rendering with the 
[RenderOptions](https://docs.fastpdfservice.com/render#render-options).

It is also possible to add custom headers and footers to your PDF documents. These headers and footers can include images, and
dynamic content such as the current page number, total number of pages, and the current date. Let's take a look at how to do this.

Firstly, let's create a new template:

<CodeGroup title="A simple template">

```C# {{ title: 'C#' }}
String template_content = """
<html>
<body>
    <h1>{{title}}</h1>
    <p>{{content}}</p>
</body>
</html>
""";
Template templateData = new Template { Name = "custom-header-footer-document", Format = "html" };
```

</CodeGroup>

You might have noticed that we have not added the template yet. It is because we need to define our header and footer first.
Headers and footers comes with [special HTML classes](https://docs.fastpdfservice.comtemplates#header-and-footer) that can be used to inject printing values into them.
We will use them to set the page number.


<CodeGroup title="Custom header and footer">

```C# {{ title: 'C#' }}
string templateContent = @"
<html>
<body>
    <h1>{{title}}</h1>
    <p>{{content}}</p>
</body>
</html>
";
Template templateData = new Template { Name = "custom-header-footer-document", Format = "html" };

string headerContent = @"
<div style='text-align: center; padding: 10px; font-size: 8px;'>
    <h2>{{header_note}}</h2>
</div>
";
string footerContent = @"
<div style='text-align: center; margin: auto; padding: 10px; font-size: 8px;'>
    <span class='pageNumber'></span>
</div>
";
Template template = await client.AddTemplateAsync(templateContent, templateData, headerContent, footerContent);
```

</CodeGroup>

Now, when you render the PDF, pass the `header_note`, `title` and `content` in your data:

<CodeGroup title="Rendering custom header document">

```C# {{ title: 'C#' }}
var documentData = new Dictionary<string, object>
{
    {"title", "My document"},
    {"content", "My document content"},
    {"header_note", "This is my favorite header"}
};

string templateId = template.Id;
byte[] document = await client.RenderTemplateAsync(templateId, documentData);
await client.SaveAsync(document, "path/to/custom-header-document.pdf");
```

</CodeGroup>


## Generating Multiple PDFs

The FastPDF API also provides the ability to generate multiple PDFs in a single API call. This feature, referred to as batch processing, can be useful in situations where you need to generate many PDFs with different data.

The way batch processing works in FastPDF is that you provide an array of data objects, each of which corresponds to one PDF. Let's see how to do this in practice.

First, let's prepare our template and data. Suppose we want to generate PDFs for each user in our system.

<CodeGroup title="Basic template">

```C# {{ title: 'C#' }}
string templateContent = @"
<html>
<body>
    <h1>Welcome, {{name}}!</h1>
    <p>Email: {{email}}</p>
</body>
</html>
";
Template templateData = new Template { Name = "image-document", Format = "html" };
Template template = await client.AddTemplateAsync(templateContent, templateData);
```

</CodeGroup>

Here, the template contains placeholders for user data (name and email). Next, we will prepare our data array.
Each element of the array corresponds to one document. Here we will render 3 documents.

<CodeGroup title="List of data objects">

```C# {{ title: 'C#' }}
var users = new List<Dictionary<string, object>>
{
    new Dictionary<string, object> { {"name", "John Doe"}, {"email", "john@example.com"} },
    new Dictionary<string, object> { {"name", "Jane Doe"}, {"email", "jane@example.com"} },
    new Dictionary<string, object> { {"name", "Richard Roe"}, {"email", "richie@example.com"} }
};
```

</CodeGroup>

Finally, we can call the `RenderTemplateManyAsync()` method with our template id and batch data.

<CodeGroup title="Rendering of multiple Documents">

```C# {{ title: 'C#' }}
string templateId = template.Id;
byte[] zipResult = await client.RenderTemplateManyAsync(templateId, users);
```

</CodeGroup>

The `RenderTemplateManyAsync()` method will return a zip file that contains the 3 PDFs. 
You now have the choice between extracting the zipResult into a C# List, or in a directory on your disk, both using the `extract()` method.

<CodeGroup title="Extracting result PDFs">

```C# {{ title: 'C#' }}
// Save all pdfs to the disk
await client.ExtractToDirectoryAsync(zipResult, "path/to/directory/");

// Or get a list of PDF
List<byte[]> pdfs = await client.ExtractAsync(zipResult);
```

</CodeGroup>



## Support

For any questions or support, reach out to us at `support@fastpdfservice.com`.

## License

fastpdf C# Package is licensed under the MIT license. See LICENSE for more details.


