# HTTP Header Analyzer

This ASP.NET Core application analyzes HTTP header data for a list of URLs, calculating various statistics such as the average age of the pages, the standard deviation of the ages, the total content length, the average content length, and the last modification date for each URL. The application also groups the URLs by their content type and counts the occurrences of each content type.

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

1. Install the [.NET SDK](https://dotnet.microsoft.com/download) (version 6.0 or later)

### Installing

1. Clone the repository to your local machine
```bash
git clone https://github.com/TobiasBonifay/HeaderHTTPproject.git
```

2. Navigate to the project folder
```bash
cd HeaderHTTPproject
```

3. Run the application
```bash
dotnet run
```

4. Open your web browser and visit `http://localhost:5000` to access the application

## Usage

1. On the homepage, you can either:
    - Select the "questionX" option to analyze the default URLs
    - Select the "your url" option to enter a list of URLs to analyze
The urls must be separated by comas (,).

3. The application can display various statistics depending on the selected option for the analyzed URLs:
    - Average age
    - Standard deviation of ages
    - Total content length
    - Average content length
    - Last modification date for each URL
    - Occurrences of each content type

## Project Structure

The project consists of several classes and files:

1. `Program.cs`: The entry point of the application, which sets up and runs the web host
2. `HeaderReaderBuilder.cs`: A class that defines methods for adding various statistics to a StringBuilder, such as average age, standard deviation of ages, total content length, average content length, and occurrences of each content type
3. `HeaderData.cs`: A class that represents the header data of a URL, including URL, age value, content length, content type, and last modification date
4. `Startup.cs`: A class that sets up the application's middleware and endpoints
5. `Calculation.cs`: A class that contains methods for calculating various statistics, such as average age, standard deviation, and retrieving important header data
6. `BestWebsites.cs`: A class that contains a list of URLs to analyze
7. `QuestionX.cs`: The specific logic for the questionX page
8. `HtmlGenerator` and `HeaderReaderBuilder`: to display the results of the analysis
...
9. `wwwroot`: A folder that contains static files such as CSS stylesheets and JavaScript file

## The original work

### Question 1

This scenario focuses on identifying the distribution of server types hosting the websites in the list of URLs.

To run this test scenario, you can use the following endpoint:
`http://localhost:5000/question1`

### Question 2

This scenario focuses on identifying the URLs with the highest and lowest content length values OF THE SAME WEBSITE, here wikipedia.
It calculates the highest and lowest content length values and presents the results in a human-readable format.

The available endpoints for this scenario is:
`http://localhost:5000/question2`

### Question 3

This scenario gives more information about the age of the web pages in the list of URLs. It calculates the average age of the pages, the standard deviation of the ages, and the last modification date for each URL.
Like the others, the available endpoints for this scenario is: 
`http://localhost:5000/question3`

## Additional Test Scenarios

This application includes three additional test scenarios that showcase various statistics and insights that can be derived from analyzing HTTP headers. These test scenarios are designed to demonstrate the flexibility and creativity of the application in processing and analyzing header data. Even though these test may be not be as useful as expected by the teacher, they are still interesting to see.

### Test Scenario 1


### Test Scenario 2

The news websites are updated frequently and contains way more content in their content. The deviation is also higher than the other websites.

### Test Scenario 3



