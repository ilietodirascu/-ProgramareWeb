#! /usr/bin/env bash
while getopts "u:s:h" opt; do
  path="$(pwd)\WebScrapper\WebScrapper"
  case $opt in
  u)
    url=$OPTARG
    dotnet run --project "$path" "u $url"
    ;;
  s)
    query=$OPTARG
    dotnet run --project "$path" "s $query"
    ;;
  h)
    echo "go2web -u <URL>         # make an HTTP request to the specified URL and print the response"
    echo "go2web -s <search-term> # make an HTTP request to search the term using your favorite search engine and print top 10 results"
    echo "go2web -h               # show this help"
    exit 1
    ;;
  esac
done
#ADD YOUR CODE HERE
