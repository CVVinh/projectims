import jwtDecode from 'jwt-decode'
import { HTTP } from '@/http-common'
import { convertToLowerCaseAndRemoveVietnameseAccents } from './conver-string'

export class checkAccessModule {
    public static token = this.setToken()
    public static listAction = []
    public static setToken() {
        var decore = JSON.stringify(
            localStorage.getItem('token') != null ? jwtDecode(localStorage.getItem('token')) : null,
        )
        if (decore != null) {
            this.token = JSON.parse(decore)
            return this.token
        }
        return null
    }

    public static getToken() {
        return this.token
    }
    public static getUserIdCurrent() {
        return Number(this.token.id)
    }
    public static getIdUserOnGitLab() {
        return Number(this.token.idUserGitLab);
    }
    public static getTokenUserOnGitLab() {
        return this.token.tokenUserGitLab;
    }
    public static getFullName() {
        return this.token.fullName
    }

    public static getAvatarLink() {
        if (this.token.avatarLink) {
            return this.token.avatarLink
        } else {
            return null
        }
    }

    public static getUserCode() {
        return this.token.userCode
    }

    public static getBranchUser() {
        return this.token.branchUser
    }

    public static getUserEmail() {
        return this.token.email
    }

    public static getListGroup() {
        if (typeof this.token.listGroup === 'string') {
            this.token.listGroup = [this.token.listGroup]
        }
        return this.token.listGroup
    }

    public static checkAccessModule = (path: any) => {
        var access = false
        var arr = []
        if (this.token.role !== undefined) {
            if (typeof this.token.role === 'string') {
                arr.push(this.token.role)
                arr.map((ele) => {
                    if (ele.includes('permission_group: True module: ' + path) === true) {
                        access = true
                    }
                })
                console.log(arr)
            } else {
                this.token.role.map((ele) => {
                    if (ele.includes('permission_group: True module: ' + path) === true) {
                        access = true
                    }
                })
            }
            return access
        } else {
            return access
        }
    }

    public static getListNameGroup = () => {
        if (typeof this.token.listNameGroup === 'string') {
            this.token.listNameGroup = [this.token.listNameGroup]
        }
        var arr = this.token.listNameGroup.map((ele: any) => {
            return ele.toLowerCase()
        })
        return arr
    }

    public static isAdmin() {
        var listNameGroup = Array.isArray(this.token.listNameGroup)
            ? this.token.listNameGroup
            : [this.token.listNameGroup]
        var newArray = listNameGroup.map((ele: any) => ele.toLowerCase())
        return newArray.includes('admin') ? true : false
    }

    public static isPm() {
        var listNameGroup = Array.isArray(this.token.listNameGroup)
            ? this.token.listNameGroup
            : [this.token.listNameGroup]
        var newArray = listNameGroup.map((ele: any) => ele.toLowerCase())
        return newArray.includes('pm') ? true : false
    }

    public static isLead() {
        var listNameGroup = Array.isArray(this.token.listNameGroup)
            ? this.token.listNameGroup
            : [this.token.listNameGroup]
        var newArray = listNameGroup.map((ele: any) => ele.toLowerCase())
        return newArray.includes('lead') ? true : false
    }

    public static isOffice() {
        var listNameGroup = Array.isArray(this.token.listNameGroup)
            ? this.token.listNameGroup
            : [this.token.listNameGroup]
        var newArray = listNameGroup.map((ele: any) => ele.toLowerCase())
        return newArray.includes('office') ? true : false
    }

    public static isStaff() {
        var listNameGroup = Array.isArray(this.token.listNameGroup)
            ? this.token.listNameGroup
            : [this.token.listNameGroup]
        var newArray = listNameGroup.map((ele: any) => ele.toLowerCase())
        return newArray.includes('staff') ? true : false
    }

    public static checksidebar(showSidebar: any) {
        var arr = []
        if (this.token.role !== undefined) {
            if (typeof this.token.role === 'string') {
                arr.push(this.token.role)
                arr.map((ele) => {
                    if (ele.includes('permission_group: True module: ') === true) {
                        showSidebar[ele.slice(31).toLowerCase()] = true
                    }
                })
            } else {
                this.token.role.map((ele: any) => {
                    if (ele.includes('permission_group: True module: ') === true) {
                        showSidebar[ele.slice(31).toLowerCase()] = true
                    }
                })
            }
        }
    }

    public static checkOperationSidebar(nameOperation: any, nameModule: any) {
        if (this.token.role.includes(`module:${nameModule} action:` + nameOperation)) {
            return true
        } else {
            return false
        }
    }
    public static checkGetData(module: any) {
        const nameModule = module.toLowerCase()
        if (nameModule === 'ots') {
            return ['admin', 'pm']
        }
        if (nameModule === 'leaveoffs/acceptregisterlists') {
            return ['admin']
        }
        if (nameModule === 'leaveoffs/summary') {
            return ['admin', 'office', 'pm']
        }
        if (nameModule === 'project') {
            return ['admin', 'office', 'pm']
        }
        if (nameModule === 'users') {
            return ['admin', 'office']
        }
        if (nameModule === 'paids') {
            return ['admin', 'office']
        }
        if (nameModule === 'tasks') {
            return ['admin', 'office']
        }
        if (nameModule === 'reviews') {
            return ['admin', 'pm']
        }
        if (nameModule === 'reviews/reviewstaffs') {
            return ['admin', 'pm']
        }
        if (nameModule === 'timesheetdailys') {
            return ['admin', 'pm', 'office']
        }
    }

    public static checkCallAPI(module: any) {
        var isGetAll = false
        this.getListNameGroup().filter((element1: any) => {
            if (this.checkGetData(module).includes(element1)) {
                isGetAll = true
            }
        })
        return isGetAll
    }

    public static checkIsNotGroup(nameGroup: any) {
        const name = nameGroup.toLowerCase()
        if (checkAccessModule.getListNameGroup().length === 1) {
            if (checkAccessModule.getListNameGroup().includes(name) === true) {
                return true
            } else {
                return false
            }
        } else {
            return false
        }
    }
    public static getlistActionGroupModule(module: any) {
        return this.token.role.filter((item: any) => item.includes(module))
    }

    public static getListAction() {
        HTTP.get('/ActionModule/getAllActionModule')
            .then((res) => {
                this.listAction = res.data._Data
            })
            .catch((err) => console.log(err))
    }

    public static checkOperation = (module: any) => {
        return this.getlistActionGroupModule(module)
    }

    public static checkAccessRights() {
        var access = false

        if (this.token.listNameGroup !== undefined) {
            access = true
        }

        return access
    }


    public static checkPermissionAction = async (module: any, showButton: any) => {
        try {
            if (this.listAction.length === 0) {
                const res = await HTTP.get('/ActionModule/getAllActionModule')
                this.listAction = res.data._Data
            }
            const filteredList = this.getlistActionGroupModule(module)
                .map((ele: any) => {
                    if (ele.includes(`module:${module} action`) === true) {
                        return ele
                    }
                })
                .filter((name: any) => name !== null && name !== undefined)
            filteredList.map((ele: any) => {
                showButton[ele.slice(ele.indexOf('action:') + 7)] = true
            })
        } catch (error) {
            console.log(error)
        }
    }
}
