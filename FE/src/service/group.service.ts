import { HTTP } from '@/http-common'
import { ApiApplication } from '../config/app.config'

export class GroupsService {
    public static filterGroupByName(name: string, pageIndex: Number, pageSize: Number) {
        return HTTP.get(ApiApplication.GROUP.SEARCH_GROUP_BY_NAME(name, pageIndex, pageSize))
    }
}
