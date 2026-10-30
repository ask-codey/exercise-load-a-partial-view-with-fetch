# Exercise: Load a Partial View with fetch

Exercise code for the lesson **Exercise: Load a Partial View with fetch** in the C# Full-Stack Bootcamp on [ask-codey.com](https://www.ask-codey.com).

BooksPlus's search page makes you type, then press **Search**, then wait for a whole new page. You'll make it search as you type: the results below the box change with every letter, and the page never reloads.

This is the BooksPlus app from *Exercise: MVC with Dapper*, finished.

## What's here

- `BooksPlusWeb`: the BooksPlus MVC app. `Views/Books/Search.cshtml`, `Controllers/BooksController.cs` and `wwwroot/js/search.js` are the files you'll change.
- `BooksPlusWeb.Tests`: tests that start the app and request its pages. You don't need to change these.

## Before you start

The app uses your own `BooksPlus` database. If yours has another name, change the connection string in `appsettings.json`.

## What to do

1. **The partial:** move the results part of `Views/Books/Search.cshtml` (the count and the list) into a new partial view, `Views/Books/_SearchResults.cshtml`. On the search page, render it with `<partial>` inside `<div id="search-results">`.
2. **The action:** add `GET /books/search/results?q=...` to `BooksController`, returning only that partial, with `PartialView`. With no text, it answers with nothing at all.
3. **The script:** give the search box the id `q`, load `wwwroot/js/search.js` in the page's `Scripts` section, and write the live search. Its requirements are in the comment at the top of `search.js`.

Run the tests with **Test > Run All Tests**. Then run the app, open the Search page, and try the checklist.

## Checklist for the page

- [ ] Typing *SQL* shows *28 books match "SQL"* and the list, without pressing Search, and without the page reloading.
- [ ] Each letter you type or delete updates the results.
- [ ] Deleting back to one letter clears the results.
- [ ] Typing *zzz* shows *0 books match "zzz"*.
- [ ] Clicking a result opens that book.
- [ ] With JavaScript turned off, or by pressing Enter, the Search button still works the old way.

## Tips

- The partial can read `ViewData["SearchText"]` too: a partial shares its page's `ViewData`, and the new action can set it as `Search` does.
- `PartialView("_SearchResults", books)` renders the partial without the layout.
- The `input` event fires on every change to a text box.
- `` `/books/search/results?q=${encodeURIComponent(text)}` `` builds a safe address for any text.
- `response.text()` reads the partial's HTML; `innerHTML` puts it in the page.

## Hand it in

When it works, push your solution to a public repo of your own on GitHub, then paste that repo's address into **Submit your exercise** on the lesson page at [ask-codey.com](https://www.ask-codey.com). That's what completes the lesson. The lesson page shows the Git commands, step by step.
