import { http, HttpResponse } from "msw"
import searchDataJson1 from './data/searchData1.json';
import searchDataJson2 from './data/searchData2.json';

interface LoginPathParams {
}

interface LoginRequestBody {
  email: string
  password: string
}

interface LoginResponseBody {
  
}

interface RegisterPathParams {

}

interface RegisterRequestBody {
  email: string
  password: string
}

export const handlers = [
  http.post<
    LoginPathParams,
    LoginRequestBody,
    LoginResponseBody
    >("/api/login", async ({ request }) => {
      const url = new URL(request.url);
      const useCookies = url.searchParams.get("useCookies");
  
      if (useCookies === "true") {
        const {email, password} = await request.clone().json()
        if (email === "error" && password === "error")
        {
          return HttpResponse.json({ }, { status: 400 });
        }

        return HttpResponse.json({ }, { status: 200 });
      }

      return new HttpResponse(null, { status: 404 });  // TODO: Maybe make this more of an error in future if needed
  }),

  http.post<
    RegisterPathParams,
    RegisterRequestBody
    >("/api/register", async ({ request }) => { 
      const {email, password} = await request.clone().json()
      if (email === "error" && password === "error")
      {
        return HttpResponse.json(
          {
            errors:
            {
              "InvalidEmail": [
                  "Email 'error' is invalid."
              ]
            }
          },
          { status: 400 }
        );
      }

      return HttpResponse.json({ }, { status: 200 });
  }),

  http.post("/api/logout", ( ) => { 
    return new HttpResponse(null, { status: 200 });
  }),

  http.get("/api/search", ({ request }) => {
    const url = new URL(request.url);
    const searchQuery = url.searchParams.get('searchQuery');
 
    switch (searchQuery) {
      case "1":
        return HttpResponse.json(searchDataJson1, { status: 200 });
      case "2":
        return HttpResponse.json(searchDataJson2, { status: 200 });
      case "empty":
        return HttpResponse.json([ ], { status: 200 });
      default:
        return HttpResponse.json({"error": "Not a valid search query for mock"}, { status: 404 });
    }
  })
]