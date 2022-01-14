export class Test {

    public Test() {
        const query: ILucenQuery = {
            "size": 10,
            "sort": { "type": { "order": "", "type": "STRING" } },
            "query": {
                "bool": {
                    "must": {
                        "term": { "aa": "11" }
                    }
                }
            }
        };
        return JSON.stringify(query);
    }

    public Test_match_all() {
        const query: ILucenQuery = {
            "size": 10,
            sort: { "Content.ContentItem.CreatedUtc": { order: "desc", type: "string" } },
            query: {
                "match_all": {}
            }
        };
        JSON.stringify(query);
    }

}



export interface ILucenQuery {

    query: {
        [key: string | "bool" | "fuzzy" | "match_all" | "match_phrase" | "match" | "prefix" | "query_string"
            | "range" | "regexp" | "simple_query_string" | "term" | "terms" | "wildcard"]
        : BooleanQuery | object;
        filter?: []
    }
    size?: number
    from?: number
    sort?: { [key: string]: LucenSort; } | Array<{ [key: string]: LucenSort; }>
}


export interface QueryFragment {

}

export class QueryFilter {
    filter: [[key: string]]
}

export interface BooleanQuery {
    [key: string | "must" | "mustnot" | "must_not" | "should" | "boost" | "minimum_should_match" | "filter"]
    : Term | Array<Term>
}


// export interface FuzzyQuery {
//     [key: string | "must" | "mustnot" | "must_not" | "should" | "boost" | "minimum_should_match" | "filter"]
//     : Term | Array<Term>
// }

export interface Wildcard {
    [key: string]:
}

export interface Term {
    [key: string]: { [key: string]: string }
}
export class LucenSort {

    order: "desc" | string = "asc"
    /**
     * "desc" if natural order should be reversed.
     */
    type: "score" | "doc" | "score" | "string" | "int32" | "single"
        | "int64" | "int64" | "double"
        | "custom" | "string_val"
        | "bytes" | "rewriteable" = "string"
    //| "INT16" //deprecated
    //| "BYTE" //deprecated
} 