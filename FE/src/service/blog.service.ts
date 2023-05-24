import { HTTP } from '@/http-common'
import { ApiApplication } from '../config/app.config'

export class BlogService {
    public static getAllBlogPageList(pageIndex: Number, pageSize: Number) {
        return HTTP.get(ApiApplication.BLOG.GET_ALL_BLOG(pageIndex, pageSize))
    }
    public static getBlogByID(id: Number) {
        return HTTP.get(ApiApplication.BLOG.GET_BLOG_BY_ID(id))
    }
    public static handlerFilterBlog(
        postId: Number,
        title: string,
        user: string,
        category: string,
        pageIndex: Number,
        pageSize: Number,
    ) {
        return HTTP.get(ApiApplication.BLOG.SEARCH_BLOG(postId, title, user, category, pageIndex, pageSize))
    }
    public static handlerReplyComment(object: any) {
        return HTTP.post(ApiApplication.BLOG.REPLY_COMMENT_OF_POST, object)
    }
}
