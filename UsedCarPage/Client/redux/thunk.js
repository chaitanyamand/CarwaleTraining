import { FETCH_PRODUCT_FAILURE, FETCH_PRODUCT_PENDING, FETCH_PRODUCT_SUCCESS } from "./actions";

export const fetchProducts = (inputUrl) => {
    return (dispatch, getState) => {
        const { filter, product } = getState();

        let url = inputUrl;
        if (!url) {
            const fuelSelected = Array.from(filter.fuelSelected);
            const [minPrice, maxPrice] = filter.priceRange;
            const fuelParam = encodeURIComponent(fuelSelected.join("+"));
            const budgetParam = `${minPrice}-${maxPrice}`;
            url = `http://localhost:5000/api/stocks?fuel=${fuelParam}&budget=${budgetParam}`;
        }

        // Abort any ongoing request
        if (product.requestController) {
            product.requestController.abort();
        }

        const controller = new AbortController();
        dispatch({ type: FETCH_PRODUCT_PENDING, payload: { requestController: controller } });

        // Set a timeout to abort the request if it takes too long
        const timeoutId = setTimeout(() => {
            if (product.requestController === controller) {
                controller.abort();
                dispatch({ type: FETCH_PRODUCT_FAILURE });
            }
        }, 3000);

        fetch(url, { signal: controller.signal })
            .then((res) => res.json())
            .then((data) => {
                clearTimeout(timeoutId);

                dispatch({ type: FETCH_PRODUCT_SUCCESS, payload: { data: data.stocks, totalNumber: data.totalCount, nextPageUrl: data.nextPageUrl, prevPageUrl: data.previousPageUrl } });
            })
            .catch((err) => {
                if (err.name !== "AbortError") {
                    console.error("Fetch error:", err);
                    dispatch({ type: FETCH_PRODUCT_FAILURE });
                }
            });
    };
};
