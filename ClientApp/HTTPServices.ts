import axios, {
    AxiosRequestConfig,
    AxiosResponse,
    AxiosError,
    AxiosInstance,
    AxiosAdapter,
    Cancel,
    CancelToken,
    CancelTokenSource,
    Canceler
} from 'axios';
import store from './store/store';
import AuthService from '@/AuthService';
//const auth = new AuthService();

export class HTTPSingleton {
    private static instance: HTTPSingleton;
    private static config: AxiosRequestConfig = {
        baseURL: process.env.VUE_APP_ROOT_API,
        headers: {
            'Content-Type': 'application/json',
            'Access-Control-Allow-Origin': '*'
        },
        timeout: 1000000,
        responseType: 'json',
        xsrfCookieName: 'XSRF-TOKEN',
        xsrfHeaderName: 'X-XSRF-TOKEN',
        onUploadProgress: (progressEvent: any) => { },
        onDownloadProgress: (progressEvent: any) => { },
        cancelToken: new axios.CancelToken((cancel: Canceler) => { })
    };
    private HTTP: AxiosInstance;
    private constructor() {
        this.HTTP = axios.create(HTTPSingleton.config);
        this.setAccessToken();
    }
    static getInstance() {
        if (!HTTPSingleton.instance) {
            HTTPSingleton.instance = new HTTPSingleton();
        }
        return HTTPSingleton.instance;
    }
    getHTTP(): AxiosInstance {
        return this.HTTP;
    }
    setAccessToken(): void {
        //this.HTTP.interceptors.request.use(async (config) => {
        //    // Do something before request is sent
        //    let userToken = await auth.getAccessToken();
        //    config.headers['Authorization'] = `Bearer ${userToken}`;
        //    return config;
        //}, function (error) {
        //    // Do something with request error
        //    return Promise.reject(error);
        //});
    }
}
export const HTTP: AxiosInstance = HTTPSingleton.getInstance().getHTTP();
export class HTTPDownloadSingleton {
    private static instance: HTTPDownloadSingleton;
    private static config: AxiosRequestConfig = {
        baseURL: process.env.VUE_APP_ROOT_API,
        headers: {
            'Content-Type': 'application/json',
            'Access-Control-Allow-Origin': '*'
        },
        timeout: 1000000,
        responseType: 'blob',
        xsrfCookieName: 'XSRF-TOKEN',
        xsrfHeaderName: 'X-XSRF-TOKEN',
        onUploadProgress: (progressEvent: any) => { },
        onDownloadProgress: (progressEvent: any) => { },
        cancelToken: new axios.CancelToken((cancel: Canceler) => { })
    };
    private HTTP: AxiosInstance;
    private constructor() {
        this.HTTP = axios.create(HTTPDownloadSingleton.config);
        this.setAccessToken();
    }
    static getInstance() {
        if (!HTTPDownloadSingleton.instance) {
            HTTPDownloadSingleton.instance = new HTTPDownloadSingleton();
        }
        return HTTPDownloadSingleton.instance;
    }
    getHTTP(): AxiosInstance {
        return this.HTTP;
    }
    setAccessToken(): void {
        //this.HTTP.interceptors.request.use(async (config) => {
        //    // Do something before request is sent
        //    let userToken = await auth.getAccessToken();
        //    config.headers['Authorization'] = `Bearer ${userToken}`;
        //    return config;
        //}, function (error) {
        //    // Do something with request error
        //    return Promise.reject(error);
        //});
        ////if (store.state.user && store.state.user.AccessToken && store.state.user.AccessToken.Token) {
        ////    this.HTTP.defaults.headers.common['access_token'] = store.state.user.AccessToken.Token;
        ////}
    }
}
export const HTTPDownload: AxiosInstance = HTTPDownloadSingleton.getInstance().getHTTP();