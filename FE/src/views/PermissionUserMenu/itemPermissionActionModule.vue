<template>
    <div class="row">
        <div class="col-sm-3" v-for="x in listaction" :key="x" >
            <input
                class="input-checkbox"
                type="checkbox"
                :id="x.actionModule.id+'-'+x.module.id"
                :value="x.actionModule.id"
                v-bind:checked="checkCheckedbox(x)"
                @change="isSelectedCheck(x.actionModule.id ,$event.target.checked)"
            />
                <label :for="x.actionModule.id+'-'+x.module.id" class="hover-checkbox">{{x.actionModule.name}}</label>
        </div>
    </div>
    <!-- <div class="d-flex align-items-center justify-content-center me-2" v-for="x in listaction" :key="x" >
        <input
            class="input-checkbox"
            type="checkbox"
            :id="x.actionModule.id+'-'+x.module.id"
            :value="x.actionModule.id"
            v-bind:checked="checkCheckedbox(x)"
            @change="isSelectedCheck(x.actionModule.id ,$event.target.checked)"
        />
            <label :for="x.actionModule.id+'-'+x.module.id" class="hover-checkbox">{{x.actionModule.name}}</label>
    </div> -->

    <!-- 
        1 = add
        2 = update  
        3 = delete
        4 = deleteMulti
        5 = confirm
        6 = confirmMulti
        7 = refuse
        8 = addMember
        9 = export excel
        -->
</template>

<script>
import { HTTP } from '@/http-common';

export default {
    data() {
        return {
            listaction : null,
            check : true,
        }
    },
    props: ['module'],
    async mounted(){
       await this.getActionByIdModule()
    },

    methods:{

        isSelectedCheck(idSelected,Checked){    
            if(Checked){
                if(idSelected === 1){
                    this.module.add ++;
                }
                if(idSelected === 2){
                    this.module.update ++;
                }
                if(idSelected === 3){
                    this.module.delete++;
                }
                if(idSelected === 4){
                    this.module.deleteMulti ++;
                }
                if(idSelected === 5){
                    this.module.confirm ++;
                }
                if(idSelected === 6){
                    this.module.confirmMulti ++;
                }
                if(idSelected === 7){
                    this.module.refuse ++;
                }
                if(idSelected === 8){
                    this.module.addMember ++;
                }
                if(idSelected === 9){
                    this.module.export ++;
                }

            }else{
                if(idSelected === 1){
                    this.module.add = --this.module.add < 0 ? 0 : this.module.add;
                }
                if(idSelected === 2){
                    this.module.update = --this.module.update < 0 ? 0 : this.module.update;
                }
                if(idSelected === 3){
                    this.module.delete = --this.module.delete < 0 ? 0 : this.module.delete;
                }
                if(idSelected === 4){
                    this.module.deleteMulti = --this.module.deleteMulti < 0 ? 0 : this.module.deleteMulti;
                }
                if(idSelected === 5){
                    this.module.confirm = --this.module.confirm < 0 ? 0 : this.module.confirm;
                }
                if(idSelected === 6){
                    this.module.confirmMulti = --this.module.confirmMulti < 0 ? 0 : this.module.confirmMulti;
                }
                if(idSelected === 7){
                    this.module.refuse = --this.module.refuse < 0 ? 0 : this.module.refuse;
                }
                if(idSelected === 8){
                    this.module.addMember = --this.module.addMember < 0 ? 0 : this.module.addMember;
                }
                if(idSelected === 9){
                    this.module.export = --this.module.export < 0 ? 0 : this.module.export;
                }
            }
        },
        async getActionByIdModule(){
            if(this.module){
                await HTTP.get("PermissionActionModule/getPermissionActionModuleWithModuleId/" + this.module.id)
                .then(res=>{
                    this.listaction = res.data._Data
                })
                .catch(err=>console.log(err))
            }
        },
        checkCheckedbox(idaction){
           if(idaction){
                if(idaction.actionModule.id === 1){
                   if(this.module.add != 0){
                    return true
                   }
                } 
                if(idaction.actionModule.id === 2){
                    if(this.module.update != 0){
                    return true
                   }
                }
                if(idaction.actionModule.id === 3){
                    if(this.module.delete != 0){
                    return true
                   }
                }
                if(idaction.actionModule.id === 4){
                    if(this.module.deleteMulti != 0){
                    return true
                   }
                }
                if(idaction.actionModule.id === 5){
                    if(this.module.confirm != 0){
                    return true
                   }
                }
                if(idaction.actionModule.id === 6){
                    if(this.module.confirmMulti != 0){
                    return true
                   }
                }
                if(idaction.actionModule.id === 7){
                    if(this.module.refuse != 0){
                      return true
                   }
                }
                if(idaction.actionModule.id === 8){
                    if(this.module.addMember != 0){
                    return true
                   }
                }
                if(idaction.actionModule.id === 9){
                    if(this.module.export != 0){
                    return true
                   }
                }
           }
        },
    }
}
</script >

<style scope lang="scss">
    .input-checkbox {
        border-radius: 6px;
        transform: scale(1.5);
        margin-right: 9px;
        cursor: pointer;
        //transition: background-color 0.2s, color 0.2s, border-color 0.2s, box-shadow 0.2s;
    }
    .hover-checkbox {
        cursor: pointer;
    }
</style>